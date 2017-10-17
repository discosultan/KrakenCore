using KrakenCore.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KrakenCore
{
    /// <summary>
    /// A strongly typed async HTTP client for Kraken bitcoin exchange API.
    /// <para>https://www.kraken.com/help/api</para>
    /// </summary>
    public partial class KrakenClient : IDisposable
    {
        public const string DummyApiKey = "00000000000000000000000000000000000000000000000000000000";
        public const string DummyPrivateKey = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
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
        /// <param name="apiKey">
        /// Key required to make private queries to the API. Use <see cref="DummyApiKey"/> if only
        /// use public API is used.
        /// </param>
        /// <param name="privateKey">
        /// Secret required to sign private messages. Use <see cref="DummyPrivateKey"/> if only
        /// public API is used.
        /// </param>
        /// <param name="rateLimit">
        /// Used to enable API call rate limiter conforming to Kraken rules. To disable, use <see cref="RateLimit.None"/>.
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

        /// <summary>
        /// Gets the API key used for private requests.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Gets the private key aka secret used to sign private requests.
        /// </summary>
        public string PrivateKey { get; }

        /// <summary>
        /// Gets the rate limit applied for this client.
        /// </summary>
        public RateLimit RateLimit { get; }

        /// <summary>
        /// Gets or sets the base address of Uniform Resource Identifier (URI) of the Kraken API used
        /// when sending requests (default = https://api.kraken.com).
        /// </summary>
        public Uri BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        /// <summary>
        /// Gets the additional headers which should be sent with each request (default = empty).
        /// </summary>
        public HttpRequestHeaders DefaultHeaders => _httpClient.DefaultRequestHeaders;

        /// <summary>
        /// Gets or sets if error strings returned by Kraken should raise exceptions (default = true).
        /// </summary>
        public bool ErrorsAsExceptions { get; set; } = true;

        /// <summary>
        /// Gets or sets if warning strings returned by Kraken should raise exceptions (default = false).
        /// </summary>
        public bool WarningsAsExceptions { get; set; }

        /// <summary>
        /// Gets or sets the getter function for a nonce (default = <c>DateTime.UtcNow.Ticks</c>).
        /// <para>API expects an increasing value for each request.</para>
        /// </summary>
        public Func<Task<long>> GetNonce { get; set; } = () => Task.FromResult(DateTime.UtcNow.Ticks);

        /// <summary>
        /// Gets or sets the getter function for a two-factor password (default = null).
        /// <para>Set to <c>null</c> to disable.</para>
        /// </summary>
        public Func<Task<string>> GetTwoFactorPassword { get; set; } // Nullable

        /// <summary>
        /// Gets or sets request interceptor prior to dispatching it (default = null).
        /// <para>Can be used to log or modify the request, for example.</para>
        /// </summary>
        public Func<HttpRequestMessage, Task<HttpRequestMessage>> InterceptRequest { get; set; } // Nullable

        /// <summary>
        /// Gets or sets response interceptor after receiving it (default = null).
        /// <para>Can be used to log or modify the response, for example.</para>
        /// </summary>
        public Func<HttpResponseMessage, Task<HttpResponseMessage>> InterceptResponse { get; set; } // Nullable

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
            string nonce = null;
            if (GetNonce != null)
            {
                nonce = (await GetNonce().ConfigureAwait(false)).ToString();
                args["nonce"] = nonce;
            }
            if (GetTwoFactorPassword != null)
            {
                args["otp"] = WebUtility.UrlEncode(await GetTwoFactorPassword().ConfigureAwait(false));
            }

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
                req = await InterceptRequest(req).ConfigureAwait(false);
            }

            // Perform the HTTP request.
            HttpResponseMessage res = await _httpClient.SendAsync(req).ConfigureAwait(false);

            // Throw for HTTP-level error.
            res.EnsureSuccessStatusCode();

            // Allow interception of response by the consumer of this client.
            if (InterceptResponse != null)
            {
                res = await InterceptResponse(res).ConfigureAwait(false);
            }

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
