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

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetTradeBalance()
        {
            var res = await _client.GetTradeBalance();
            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetOpenOrders()
        {
            var res = await _client.GetOpenOrders(true);
            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetClosedOrders()
        {
            var res = await _client.GetClosedOrders(true);
            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task QueryOrdersInfo()
        {
            //var res = await _client.QueryOrdersInfo("", true);
            //Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetTradesHistory()
        {
            var res = await _client.GetTradesHistory(includeTrades: true);
            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task QueryTradesInfo()
        {
            //var res = await _client.QueryTradesInfo("");
            //Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetOpenPositions()
        {
            //var res = await _client.GetOpenPositions("", true);
            //Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetLedgersInfo()
        {
            var res = await _client.GetLedgersInfo();
            Assert.Empty(res.Errors);
            AssertNotDefault(res.Result.Count);
            AssertNotDefault(res.Result.Ledgers);
        }

        [Fact]
        public async Task QueryLedgers()
        {
            //var res = await _client.QueryLedgers();
        }

        [Fact]
        public async Task GetTradeVolume()
        {
            var res = await _client.GetTradeVolume(includeFeeInfo: true);
            Assert.Empty(res.Errors);
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