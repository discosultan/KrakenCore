using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using Xunit;

namespace KrakenCore.Tests
{
    public partial class KrakenClientTests : IDisposable
    {
        private readonly KrakenClient _client;

        public KrakenClientTests()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot config = configBuilder.Build();

            string apiKey = config["ApiKey"];

            string privateKey = config["PrivateKey"];

            if (!Enum.TryParse(config["AccountTier"], out RateLimit rateLimit))
                rateLimit = RateLimit.None;

            _client = new KrakenClient(apiKey, privateKey, rateLimit)
            {
                WarningsAsExceptions = true
            };
        }

        public void Dispose() => _client.Dispose();

        [DebuggerHidden]
        private void AssertNotDefault<T>(T value) => Assert.NotEqual(default(T), value);
    }
}