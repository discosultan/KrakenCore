using KrakenCore.Models;
using KrakenCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KrakenCore
{
    /// <summary>
    /// A strongly typed async http client for Kraken Bitcoin Exchange API.
    /// <para>https://www.kraken.com/help/api</para>
    /// </summary>
    public partial class KrakenClient : IDisposable
    {
        static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new SnakeCasePropertyNamesContractResolved()
        };

        static readonly Dictionary<AccountTier, (int Limit, TimeSpan DecreaseTime)> TierInfo
            = new Dictionary<AccountTier, (int, TimeSpan)>(3)
            {
                [AccountTier.Tier2] = (15, TimeSpan.FromSeconds(3)),
                [AccountTier.Tier3] = (20, TimeSpan.FromSeconds(2)),
                [AccountTier.Tier4] = (20, TimeSpan.FromSeconds(1))
            };

        readonly HttpClient _httpClient = new HttpClient();
        readonly string _publicApiBaseUrl;
        readonly string _privateApiBaseUrl;

        readonly HMACSHA512 _sha512PrivateKey;
        readonly SHA256 _sha256 = SHA256.Create();

        readonly RateLimiter _rateLimiter; // Nullable.

        readonly Func<long> _getNonce;

        /// <summary>
        /// Initializes a new instance of the <see cref="KrakenClient"/> class.
        /// </summary>
        /// <param name="apiKey">Key required to make queries to the API.</param>
        /// <param name="accountTier">
        /// Account tier at Kraken Exchange. This is used to enable API call rate limiter. To
        /// disable, use <see cref="AccountTier.Unknown"/>.
        /// </param>
        /// <param name="apiBaseUrl">Base address of the API including API version.</param>
        /// <param name="getNonce">
        /// Getter function for a nonce.
        /// <para>API expects an increasing value for each request.</para>
        /// <para>By default uses <c>DateTime.UtcNow.Ticks</c>.</para>
        /// </param>
        public KrakenClient(
            string apiKey,
            string privateKey,
            AccountTier accountTier = AccountTier.Unknown,
            string apiBaseUrl = "https://api.kraken.com/0",
            Func<long> getNonce = null)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));

            AccountTier = accountTier;
            if (TierInfo.TryGetValue(accountTier, out var info))
                _rateLimiter = new RateLimiter(info.Limit, info.DecreaseTime, new Stopwatch());

            if (apiBaseUrl == null) throw new ArgumentNullException(nameof(apiBaseUrl));
            _publicApiBaseUrl = apiBaseUrl + "/public";
            _privateApiBaseUrl = apiBaseUrl + "/private";

            _sha512PrivateKey = new HMACSHA512(Convert.FromBase64String(privateKey));

            _getNonce = getNonce ?? (() => DateTime.UtcNow.Ticks);
        }

        public string ApiKey { get; }

        public string PrivateKey { get; }

        public AccountTier AccountTier { get; }

        /// <summary>
        /// Sends a GET request to the Kraken API as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">Type of data contained in the response.</typeparam>
        /// <param name="requestUrl">The relative uri the request is sent to.</param>
        /// <param name="args">Optional argument passed as a query string.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requestUrl"/> is <c>null</c>.</exception>
        public async Task<KrakenResponse<T>> QueryPublic<T>(string requestUrl, Dictionary<string, string> args = null)
        {
            if (requestUrl == null) throw new ArgumentNullException(nameof(requestUrl));

            string absoluteUrl = _publicApiBaseUrl + requestUrl;

            var req = new HttpRequestMessage(HttpMethod.Post, absoluteUrl)
            {
                Content = new FormUrlEncodedContent(
                    args?.Where(arg => arg.Value != null) ?? Enumerable.Empty<KeyValuePair<string, string>>()
                )
            };
            //long nonce = _getNonce();
            //byte[] dataBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(nonce.ToString()));

            //var buffer = new byte[absoluteUrlBytes.Length + dataBytes.Length];
            //Buffer.BlockCopy(absoluteUrlBytes, 0, buffer, 0, absoluteUrlBytes.Length);
            //Buffer.BlockCopy(dataBytes, 0, buffer, absoluteUrlBytes.Length, dataBytes.Length);

            //string signature = Convert.ToBase64String(_sha512ApiKey.ComputeHash(buffer));
            //req.Headers.Add("API-Sign", signature);

            await _rateLimiter.WaitAccess(1).ConfigureAwait(false);

            HttpResponseMessage res = await _httpClient.SendAsync(req).ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
                throw new Exception($"Http request failed.\n\n{req}\n\n{res}");

            string jsonContent = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<KrakenResponse<T>>(jsonContent, JsonSettings);
        }

        public async Task<KrakenResponse<T>> QueryPrivate<T>(string requestUrl, Dictionary<string, string> args = null)
        {
            if (requestUrl == null) throw new ArgumentNullException(nameof(requestUrl));

            string absoluteUrl = _publicApiBaseUrl + requestUrl;
            byte[] absoluteUrlBytes = Encoding.UTF8.GetBytes(absoluteUrl);

            long nonce = _getNonce();
            //args = args ?? new Dictionary<string, object>();
            //args.Add("nonce", nonce);
            // Add otp if two-factor enabled.

            var req = new HttpRequestMessage(HttpMethod.Post, absoluteUrl)
            {
                Content = new FormUrlEncodedContent(
                    args?.Where(arg => arg.Value != null) ?? Enumerable.Empty<KeyValuePair<string, string>>()
                )
            };
            //string json = JsonConvert.SerializeObject(args, JsonSettings);
            //string json = JsonConvert.SerializeObject(
            //    new Dictionary<string, object>(1) { ["nonce"] = nonce },
            //    JsonSettings);

            //req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            byte[] dataBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(nonce.ToString() /*+ json*/));

            var buffer = new byte[absoluteUriBytes.Length + dataBytes.Length];
            Buffer.BlockCopy(absoluteUriBytes, 0, buffer, 0, absoluteUriBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, buffer, absoluteUriBytes.Length, dataBytes.Length);

            _httpClient.DefaultRequestHeaders.Add("API-Key", ApiKey);

            string signature = Convert.ToBase64String(_sha512PrivateKey.ComputeHash(buffer));
            req.Headers.Add("API-Sign", signature);

            HttpResponseMessage res = await _httpClient.SendAsync(req).ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
                throw new Exception($"Http request failed.\n\n{req}\n\n{res}");

            string jsonContent = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<KrakenResponse<T>>(jsonContent, JsonSettings);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}
