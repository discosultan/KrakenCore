using KrakenCore.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests
{
    // These tests DO NOT perform any monetary transactions! Add standard order call is only made in
    // validation mode.
    //
    // When testing for private API, there are a couple of prerequisites that need
    // to be met for the account under test:
    //
    // * At least one currency must be available on the balance
    // * At least one transaction must be made

    public partial class KrakenClientTests
    {
        [Fact]
        public async Task GetAccountBalance()
        {
            var res = await _client.GetAccountBalance();

            var balance = res.Result.First();
            AssertNotDefault(balance.Key);
            AssertNotDefault(balance.Value);
        }

        [Fact]
        public async Task GetTradeBalance()
        {
            var res = await _client.GetTradeBalance();

            //AssertNotDefault(res.Result.CostBasis);
            AssertNotDefault(res.Result.Equity);
            AssertNotDefault(res.Result.EquivalentBalance);
            //AssertNotDefault(res.Result.FloatingValuation);
            AssertNotDefault(res.Result.FreeMargin);
            //AssertNotDefault(res.Result.MarginAmount);
            //AssertNotDefault(res.Result.MarginLevel);
            AssertNotDefault(res.Result.TradeBalance);
            //AssertNotDefault(res.Result.UnrealizedProfitAndLoss);
        }

        [Fact]
        public async Task GetOpenOrders()
        {
            var res = await _client.GetOpenOrders(true);

            // Only assert content if any open order present.
            if (res.Result.Any())
            {
                var order = res.Result.First();
                AssertNotDefault(order.Key);
                AssertNotDefault(order.Value.OpenTime);
                Assert.Equal(OrderInfo.StatusOpen, order.Value.Status);
                AssertOrderDescription(order.Value.Description);
            }
        }

        [Fact]
        public async Task GetClosedOrders()
        {
            var res = await _client.GetClosedOrders(true);

            // Only assert content if any closed order present.
            if (res.Result.Any())
            {
                var order = res.Result.First();
                AssertNotDefault(order.Key);
                AssertNotDefault(order.Value.CloseTime);
                Assert.Equal(OrderInfo.StatusClosed, order.Value.Status);
                AssertOrderDescription(order.Value.Description);
            }
        }

        [Fact]
        public async Task QueryOrdersInfo()
        {
            var res = await _client.QueryOrdersInfo("OKCXVF-PL2ST-N3XOPQ", true); // TODO: query trans id instead

            var order = res.Result.First();

            AssertNotDefault(order.Key);
            AssertNotDefault(order.Value.Status);
            AssertOrderDescription(order.Value.Description);
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

        private void AssertOrderDescription(OrderDescription desc)
        {
            AssertNotDefault(desc.Pair);
            AssertNotDefault(desc.Type);
            AssertNotDefault(desc.Price);
            AssertNotDefault(desc.Leverage);
            AssertNotDefault(desc.Order);
        }
    }
}