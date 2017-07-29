using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using Xunit;

namespace KrakenCore.Tests
{
    public partial class KrakenClientTests : IDisposable
    {
        readonly KrakenClient _client;

        public KrakenClientTests()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot config = configBuilder.Build();

            string apiKey = config["ApiKey"];

            if (!Enum.TryParse(config["AccountTier"], out AccountTier accountTier))
                accountTier = AccountTier.Unknown;

            _client = new KrakenClient(apiKey, accountTier);
        }

        public void Dispose() => _client.Dispose();


        [DebuggerHidden]
        void AssertNotDefault<T>(T value) => Assert.NotEqual(default(T), value);
    }
}
