using System;
using System.Diagnostics;
using Xunit;

namespace KrakenCore.Tests
{
    public partial class KrakenClientTests : IDisposable
    {
        const string ApiKey = "<INSERT_KEY>";

        readonly KrakenClient _client = new KrakenClient(ApiKey);

        public KrakenClientTests() => _client = new KrakenClient(ApiKey);
        public void Dispose() => _client.Dispose();


        [DebuggerHidden]
        void AssertNotDefault<T>(T value) => Assert.NotEqual(default(T), value);
    }
}
