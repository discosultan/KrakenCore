using KrakenCore.Models;
using KrakenCore.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KrakenCore
{
    /// <summary>
    /// A strongly typed async http client for Kraken bitcoin exchange API.
    /// <para>https://www.kraken.com/help/api</para>
    /// </summary>
    public partial class KrakenClient : IDisposable
    {
        internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
            // Ignore deserializing count properties, since .NET collections already expose that.
            //Error = (obj, args) => args.ErrorContext.Handled = args.ErrorContext.Member.ToString() == "count"
        };

        private static readonly Dictionary<string, string> EmptyDictionary = new Dictionary<string, string>(0);

        private static readonly Dictionary<RateLimit, (int Limit, TimeSpan DecreaseTime)> TierInfo
            = new Dictionary<RateLimit, (int, TimeSpan)>(3)
            {
                [RateLimit.Tier2] = (15, TimeSpan.FromSeconds(3)),
                [RateLimit.Tier3] = (20, TimeSpan.FromSeconds(2)),
                [RateLimit.Tier4] = (20, TimeSpan.FromSeconds(1))
            };

        private const int AdditionalPrivateQueryArgs = 2;

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly HMACSHA512 _sha512PrivateKey;
        private readonly SHA256 _sha256 = SHA256.Create();

        private readonly RateLimiter _publicApiRateLimiter;  // Nullable.
        private readonly RateLimiter _privateApiRateLimiter; // Nullable.

        /// <summary>
        /// Initializes a new instance of the <see cref="KrakenClient"/> class.
        /// </summary>
        /// <param name="apiKey">Key required to make private queries to the API.</param>
        /// <param name="privateKey">Secret required to sign private messages.</param>
        /// <param name="rateLimit">
        /// Used to enable API call rate limiter conforming to Kraken rules. To disable, use <see cref="KrakenCore.RateLimit.None"/>.
        /// </param>
        public KrakenClient(
            string apiKey,
            string privateKey,
            RateLimit rateLimit = RateLimit.None)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));

            RateLimit = rateLimit;
            if (TierInfo.TryGetValue(rateLimit, out var info))
            {
                _privateApiRateLimiter = new RateLimiter(info.Limit, info.DecreaseTime, new Stopwatch());
                // If private rate limiter enabled, also enable public rate limiter.
                _publicApiRateLimiter = new RateLimiter(20, TimeSpan.FromSeconds(1), new Stopwatch());
            }

            _httpClient.BaseAddress = new Uri("https://api.kraken.com");

            _sha512PrivateKey = new HMACSHA512(Convert.FromBase64String(privateKey));
        }

        public string ApiKey { get; }

        public string PrivateKey { get; }

        public RateLimit RateLimit { get; }

        public Uri BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        public HttpRequestHeaders DefaultHeaders => _httpClient.DefaultRequestHeaders;

        public bool ErrorsAsExceptions { get; set; } = true;

        public bool WarningsAsExceptions { get; set; }

        /// <summary>
        /// Gets or sets the getter function for a nonce.
        /// <para>API expects an increasing value for each request.</para>
        /// <para>By default uses <c>DateTime.UtcNow.Ticks</c>.</para>
        /// </summary>
        public Func<long> GetNonce { get; set; } = () => DateTime.UtcNow.Ticks;

        public Func<HttpRequestMessage, Task> InterceptRequest { get; set; } // Nullable

        public Func<HttpResponseMessage, Task> InterceptResponse { get; set; } // Nullable

        /// <summary>
        /// Sends a public POST request to the Kraken API as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">Type of data contained in the response.</typeparam>
        /// <param name="requestUrl">The relative url the request is sent to.</param>
        /// <param name="args">Optional argument passed as form data.</param>
        /// <param name="apiCallCost">Cost of the query. Used to limit API calling rate.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requestUrl"/> is <c>null</c>.</exception>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public async Task<KrakenResponse<T>> QueryPublic<T>(
            string requestUrl,
            Dictionary<string, string> args = null,
            int apiCallCost = 1)
        {
            if (requestUrl == null) throw new ArgumentNullException(nameof(requestUrl));

            args = args ?? EmptyDictionary;

            // Setup request.
            string urlEncodedArgs = UrlEncode(args);
            var req = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(urlEncodedArgs, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            // Send request and deserialize response.
            return await SendRequest<T>(req, _publicApiRateLimiter, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a private POST request to the Kraken API as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">Type of data contained in the response.</typeparam>
        /// <param name="requestUrl">The relative url the request is sent to.</param>
        /// <param name="args">Optional arguments passed as form data.</param>
        /// <param name="apiCallCost">Cost of the query. Used to limit API calling rate.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requestUrl"/> is <c>null</c>.</exception>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public async Task<KrakenResponse<T>> QueryPrivate<T>(
            string requestUrl,
            Dictionary<string, string> args = null,
            int apiCallCost = 1)
        {
            if (requestUrl == null) throw new ArgumentNullException(nameof(requestUrl));

            args = args ?? new Dictionary<string, string>(AdditionalPrivateQueryArgs);

            // Add additional args.
            string nonce = (GetNonce?.Invoke() ?? DateTime.UtcNow.Ticks).ToString();
            args["nonce"] = nonce;
            // TODO: Add otp if two-factor enabled.

            // Setup request.
            string urlEncodedArgs = UrlEncode(args);
            var req = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(urlEncodedArgs, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            // Add API key header.
            req.Headers.Add("API-Key", ApiKey);

            // Add content signature header.
            byte[] urlBytes = Encoding.UTF8.GetBytes(requestUrl);
            byte[] dataBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(nonce + urlEncodedArgs));

            var buffer = new byte[urlBytes.Length + dataBytes.Length];
            Buffer.BlockCopy(urlBytes, 0, buffer, 0, urlBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, buffer, urlBytes.Length, dataBytes.Length);
            byte[] signature = _sha512PrivateKey.ComputeHash(buffer);

            req.Headers.Add("API-Sign", Convert.ToBase64String(signature));

            // Send request and deserialize response.
            return await SendRequest<T>(req, _privateApiRateLimiter, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the
        /// underlying <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose() => _httpClient.Dispose();

        private async Task<KrakenResponse<T>> SendRequest<T>(HttpRequestMessage req, RateLimiter rateLimiter, int cost)
        {
            // Wait before sending the request if rate limiter is enabled and counter is full.
            if (rateLimiter != null && cost > 0)
            {
                await rateLimiter.WaitAccess(cost).ConfigureAwait(false);
            }

            // Allow interception of request by the consumer of this client.
            if (InterceptRequest != null)
            {
                await InterceptRequest(req).ConfigureAwait(false);
            }

            // Perform the HTTP request.
            HttpResponseMessage res = await _httpClient.SendAsync(req).ConfigureAwait(false);

            // Allow interception of response by the consumer of this client.
            if (InterceptResponse != null)
            {
                await InterceptResponse(res).ConfigureAwait(false);
            }

            // Throw for HTTP-level error.
            res.EnsureSuccessStatusCode();

            // Deserialize response.
            string jsonContent = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<KrakenResponse<T>>(jsonContent, JsonSettings);

            // Throw for API-level error and warning if configured.
            if (result.Errors.Any(x =>
                ErrorsAsExceptions && x.SeverityCode == ErrorString.SeverityCodeError ||
                WarningsAsExceptions && x.SeverityCode == ErrorString.SeverityCodeWarning))
            {
                throw new KrakenException(result.Errors, "There was a problem with a response from Kraken.");
            }

            return result;
        }

        private static string UrlEncode(Dictionary<string, string> args) => string.Join(
            "&",
            args.Where(x => x.Value != null).Select(x => x.Key + "=" + x.Value)
        );
    }
}