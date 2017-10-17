using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace KrakenCore.Tests
{
    // Share the Kraken client between tests in order to respect API rate limits.
    public class KrakenFixture : IDisposable
    {
        public const string ApiKey = "<INSERT_API_KEY>";
        public const string PrivateKey = "<INSERT_PRIVATE_KEY>";
        public const RateLimit ApiRateLimit = RateLimit.Tier2;

        public KrakenFixture()
        {
            if (ApiKey.Length != KrakenClient.DummyApiKey.Length ||
                PrivateKey.Length != KrakenClient.DummyPrivateKey.Length)
            {
                throw new InvalidOperationException(
$@"Please configure {nameof(ApiKey)} and {nameof(PrivateKey)} in {nameof(KrakenFixture)}!
Use {nameof(KrakenClient)}.{nameof(KrakenClient.DummyApiKey)} and {nameof(KrakenClient)}.{nameof(KrakenClient.DummyPrivateKey)} to test only public API.");
            }

            Client = new KrakenClient(ApiKey, PrivateKey, ApiRateLimit)
            {
                ErrorsAsExceptions = true,
                WarningsAsExceptions = true,
                // If the API key has two factor password enabled, set the line below to return it.
                //GetTwoFactorPassword = () => Task.FromResult("<INSERT_PASSWORD>")
            };
        }

        public KrakenClient Client { get; }

        public void Dispose() => Client.Dispose();
    }

    public partial class KrakenClientTests : IClassFixture<KrakenFixture>
    {
        private readonly KrakenClient _client;

        public KrakenClientTests(ITestOutputHelper output, KrakenFixture fixture)
        {
            _client = fixture.Client;

            // Log request and response for each test.
            _client.InterceptRequest = async req =>
            {
                output.WriteLine("REQUEST");
                output.WriteLine(req.ToString());
                string content = await req.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content)) output.WriteLine(content);
                return req;
            };
            _client.InterceptResponse = async res =>
            {
                output.WriteLine("");
                output.WriteLine("RESPONSE");
                output.WriteLine(res.ToString());
                string content = await res.Content.ReadAsStringAsync();
                output.WriteLine(JToken.Parse(content).ToString(Formatting.Indented));
                return res;
            };
        }

        [DebuggerStepThrough]
        private void AssertNotDefault<T>(T value) => Assert.NotEqual(default(T), value);
    }
}
