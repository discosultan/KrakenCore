using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests
{
    public partial class KrakenClientTests
    {
        [Fact]
        public async Task GetAccountBalance()
        {
            var res = await _client.GetAccountBalance();

            Assert.NotEmpty(res.Result);
        }

        [Fact]
        public async Task GetTradeBalance()
        {
            var res = await _client.GetTradeBalance();

            AssertNotDefault(res.Result);
        }

        [Fact]
        public async Task GetOpenOrders()
        {
            var res = await _client.GetOpenOrders(true);
        }

        [Fact]
        public async Task GetClosedOrders()
        {
            var res = await _client.GetClosedOrders(true);
        }

        [Fact]
        public async Task QueryOrdersInfo()
        {
            var res = await _client.QueryOrdersInfo("", true);
        }

        [Fact]
        public async Task GetTradesHistory()
        {
            var res = await _client.GetTradesHistory(includeTrades: true);
        }

        [Fact]
        public async Task QueryTradesInfo()
        {
            var res = await _client.QueryTradesInfo("");
        }

        [Fact]
        public async Task GetOpenPositions()
        {
            var res = await _client.GetOpenPositions("", true);
        }

        [Fact]
        public async Task GetLedgersInfo()
        {
            var res = await _client.GetLedgersInfo();

            AssertNotDefault(res.Result.Count);
            AssertNotDefault(res.Result.Ledgers);
        }

        [Fact]
        public async Task QueryLedgers()
        {
            var res = await _client.QueryLedgers("");
        }

        [Fact]
        public async Task GetTradeVolume()
        {
            var res = await _client.GetTradeVolume(includeFeeInfo: true);
        }

        [Fact]
        public async Task AddStandardOrder()
        {
            //var res = await _client.AddStandardOrder(validate: true);
        }

        [Fact]
        public async Task CancelOpenOrder()
        {
            //var res = _client.CancelOpenOrder();
        }
    }
}