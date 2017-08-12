using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace KrakenCore.Tests
{
    [TestCaseOrderer("KrakenCore.Tests.Utils.PriorityOrderer", "KrakenCore.Tests")]
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

    // Share the Kraken client between tests in order to respect API rate limits.
    public class KrakenFixture : IDisposable
    {
        public KrakenFixture()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot config = configBuilder.Build();

            string apiKey = config["ApiKey"];

            string privateKey = config["PrivateKey"];

            if (!Enum.TryParse(config["RateLimit"], out RateLimit rateLimit))
                rateLimit = RateLimit.None;

            Client = new KrakenClient(apiKey, privateKey, rateLimit)
            {
                ErrorsAsExceptions = true,
                WarningsAsExceptions = true,
                // If the API key has two factor password enabled, set the line below to return it.
                //GetTwoFactorPassword = () => Task.FromResult("<PASSWORD>")
            };
        }

        public KrakenClient Client { get; }

        public void Dispose() => Client.Dispose();
    }
}