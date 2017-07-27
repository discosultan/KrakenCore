using KrakenCore.Models;
using KrakenCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new SnakeCasePropertyNamesContractResolved()
        };

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly HMACSHA512 _sha512ApiKey;
        private readonly SHA256 _sha256 = SHA256.Create();
        private readonly Func<long> _getNonce;

        /// <summary>
        /// Initializes a new instance of the <see cref="KrakenClient"/> class.
        /// </summary>
        /// <param name="apiKey">Key required to make queries to the API.</param>
        /// <param name="baseAddress">Base address of the API.</param>
        /// <param name="getNonce">
        /// Getter function for a nonce.
        /// <para>API expects an increasing value for each request.</para>
        /// <para>By default uses <c>DateTime.UtcNow.Ticks</c>.</para>
        /// </param>
        public KrakenClient(
            string apiKey,
            string baseAddress = "https://api.kraken.com",
            Func<long> getNonce = null)
        {
            ApiKey = apiKey      ?? throw new ArgumentNullException(nameof(apiKey));
            _getNonce = getNonce ?? (() => DateTime.UtcNow.Ticks);

            _sha512ApiKey = new HMACSHA512(Convert.FromBase64String(apiKey));
            _httpClient.DefaultRequestHeaders.Add("API-Key", apiKey);
            _httpClient.BaseAddress = new Uri(baseAddress ?? throw new ArgumentNullException(nameof(baseAddress)));
        }

        public string ApiKey { get; }

        /// <summary>
        /// Sends a GET request to the Kraken API as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">Type of data contained in the response.</typeparam>
        /// <param name="requestUri">The relative uri the request is sent to.</param>
        /// <param name="args">Optional argument passed as a query string.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requestUri"/> is <c>null</c>.</exception>
        public async Task<KrakenResponse<T>> Get<T>(string requestUri, Dictionary<string, object> args = null)
        {
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            requestUri = requestUri + args?.ToQueryString();

            var req = new HttpRequestMessage(HttpMethod.Get, requestUri);

            string absoluteUri = _httpClient.BaseAddress + requestUri;
            byte[] absoluteUriBytes = Encoding.UTF8.GetBytes(absoluteUri);

            long nonce = _getNonce();
            byte[] dataBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(nonce.ToString()));

            var buffer = new byte[absoluteUriBytes.Length + dataBytes.Length];
            Buffer.BlockCopy(absoluteUriBytes, 0, buffer, 0, absoluteUriBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, buffer, absoluteUriBytes.Length, dataBytes.Length);

            string signature = Convert.ToBase64String(_sha512ApiKey.ComputeHash(buffer));
            req.Headers.Add("API-Sign", signature);

            HttpResponseMessage res = await _httpClient.SendAsync(req).ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
                throw new Exception($"Http request failed.\n\n{req}\n\n{res}");

            string jsonContent = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<KrakenResponse<T>>(jsonContent, JsonSettings);
        }

        public async Task<KrakenResponse<T>> Post<T>(string requestUri, Dictionary<string, object> args = null)
        {
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            requestUri = requestUri + args?.ToQueryString();

            long nonce = _getNonce();
            //args = args ?? new Dictionary<string, object>();
            //args.Add("nonce", nonce);
            // Add otp if two-factor enabled.

            var req = new HttpRequestMessage(HttpMethod.Post, requestUri);
            //string json = JsonConvert.SerializeObject(args, JsonSettings);
            //string json = JsonConvert.SerializeObject(
            //    new Dictionary<string, object>(1) { ["nonce"] = nonce },
            //    JsonSettings);

            //req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            string absoluteUri = _httpClient.BaseAddress + requestUri;
            byte[] absoluteUriBytes = Encoding.UTF8.GetBytes(absoluteUri);

            byte[] dataBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(nonce + json));

            var buffer = new byte[absoluteUriBytes.Length + dataBytes.Length];
            Buffer.BlockCopy(absoluteUriBytes, 0, buffer, 0, absoluteUriBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, buffer, absoluteUriBytes.Length, dataBytes.Length);

            string signature = Convert.ToBase64String(_sha512ApiKey.ComputeHash(buffer));
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
