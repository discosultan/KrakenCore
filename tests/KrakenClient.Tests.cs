using KrakenCore.Tests.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace KrakenCore.Tests
{
    public abstract class KrakenClientTests : IClassFixture<KrakenClientTests.KrakenFixture>
    {
        // *******************************************//
        // Account specific configuration starts here //
        // *******************************************//

        private const string ApiKey = "";     // <<--- INSERT API KEY HERE!
        private const string PrivateKey = ""; // <<--- INSERT PRIVATE KEY HERE!

        // A rate limiter is used in tests to ensure we don't hit the Kraken API rate limit when
        // running all the tests in parallel. This should match the account's tier.
        private static readonly RateLimit PrivateApiRateLimit = RateLimit.Tier2;

        // To configure two-factor authentication, see KrakenClient construction below.

        // *****************************************//
        // Account specific configuration ends here //
        // *****************************************//

        protected const string DefaultBase = "XETH";
        protected const string DefaultBaseAlternate = "ETH";
        protected const string DefaultQuote = "ZEUR";
        protected const string DefaultPair = DefaultBase + DefaultQuote;
        protected const string DefaultPairAlternate = "ETHEUR";

        protected KrakenClientTests(ITestOutputHelper output, KrakenFixture fixture)
        {
            Client = new KrakenClient(ApiKey, PrivateKey)
            {
                // If the API key has two factor password enabled, set the line below to return it.
                //GetTwoFactorPassword = () => Task.FromResult("<INSERT_PASSWORD>")

                ErrorsAsExceptions = true,
                WarningsAsExceptions = true,

                // Log request and response for each test.
                InterceptRequest = async req =>
                {
                    output.WriteLine("REQUEST");
                    output.WriteLine(req.HttpRequest.ToString());
                    string content = await req.HttpRequest.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content)) output.WriteLine(content);

                    // Wait if we have hit the API rate limit.
                    RateLimiter limiter = req.HttpRequest.RequestUri.OriginalString.Contains("/private/")
                        ? fixture.PrivateApiRateLimiter
                        : fixture.PublicApiRateLimiter;

                    await limiter.WaitAccess(req.ApiCallCost);
                },
                InterceptResponse = async res =>
                {
                    output.WriteLine("");
                    output.WriteLine("RESPONSE");
                    output.WriteLine(res.HttpResponse.ToString());
                    string content = await res.HttpResponse.Content.ReadAsStringAsync();
                    output.WriteLine(JToken.Parse(content).ToString(Formatting.Indented));
                }
            };
        }

        protected KrakenClient Client { get; }

        [DebuggerStepThrough]
        protected void AssertNotDefault<T>(T value) => Assert.NotEqual(default(T), value);

        // Share the fixture between tests in order to respect API rate limits.
        public class KrakenFixture
        {
            public KrakenFixture()
            {
                // Public API rate limiter is not dependent on account tier.
                PublicApiRateLimiter = new RateLimiter(RateLimit.Tier4);
                // Account tier only applies to private limiter!
                PrivateApiRateLimiter = new RateLimiter(PrivateApiRateLimit);
            }

            public RateLimiter PublicApiRateLimiter { get; }
            public RateLimiter PrivateApiRateLimiter { get; }
        }
    }
}
