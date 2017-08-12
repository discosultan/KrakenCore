using KrakenCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests
{
    // These tests DO NOT perform any monetary transactions!
    // - Add standard order is tested only in validation mode
    // - For cancel open order, only failing path is tested

    public partial class KrakenClientTests
    {
        [Fact]
        public async Task GetAccountBalance()
        {
            var res = await _client.GetAccountBalance();

            if (res.Result.Any())
            {
                var balance = res.Result.First();
                AssertNotDefault(balance.Key);
                AssertNotDefault(balance.Value);
            }
        }

        [Fact]
        public async Task GetTradeBalance()
        {
            // Note that this test may fail with an internal server error when using an account with
            // empty balance. This seems to be a bug on Kraken side.

            var res = await _client.GetTradeBalance();

            AssertNotDefault(res.Result.Equity);
            AssertNotDefault(res.Result.EquivalentBalance);
            AssertNotDefault(res.Result.FreeMargin);
            AssertNotDefault(res.Result.TradeBalance);
        }

        [Fact]
        public async Task GetOpenOrders()
        {
            var res = await _client.GetOpenOrders(true);

            var orders = res.Result.Open;
            if (orders.Any())
            {
                var order = orders.First();
                AssertNotDefault(order.Key);
                AssertOrderInfo(order.Value);
                Assert.Equal(OrderInfo.StatusOpen, order.Value.Status);
            }
        }

        [Fact]
        public async Task GetClosedOrders()
        {
            var res = await _client.GetClosedOrders(true);

            if (res.Result.Closed.Any())
            {
                AssertNotDefault(res.Result.Count);
                var order = res.Result.Closed.First();
                AssertNotDefault(order.Key);
                AssertOrderInfo(order.Value);
                AssertNotDefault(order.Value.CloseTime);
            }
        }

        [Fact]
        public async Task QueryOrdersInfo()
        {
            string transactionId = await TryGetOrderId();
            if (transactionId == null) return;

            var res = await _client.QueryOrdersInfo(transactionId, true);

            var order = res.Result.First();

            AssertNotDefault(order.Key);
            AssertOrderInfo(order.Value);
        }

        [Fact]
        public async Task GetTradesHistory()
        {
            var res = await _client.GetTradesHistory(includeTrades: true);

            if (res.Result.Trades.Any())
            {
                AssertNotDefault(res.Result.Count);
                var trade = res.Result.Trades.First();
                AssertNotDefault(trade.Key);
                AssertTradeInfo(trade.Value);
            }
        }

        [Fact]
        public async Task QueryTradesInfo()
        {
            string transactionId = await TryGetTradeId();
            if (transactionId == null) return;

            var res = await _client.QueryTradesInfo(transactionId, true);

            var tradesInfo = res.Result.First(x => x.Key == transactionId).Value;
            AssertNotDefault(tradesInfo.OrderTransactionId);
            AssertNotDefault(tradesInfo.Time);
            AssertNotDefault(tradesInfo.Type);
            AssertNotDefault(tradesInfo.OrderType);
            AssertNotDefault(tradesInfo.Price);
            AssertNotDefault(tradesInfo.Cost);
            AssertNotDefault(tradesInfo.Fee);
            AssertNotDefault(tradesInfo.Volume);
        }

        [Fact]
        public async Task GetOpenPositions()
        {
            string transactionId = await TryGetTradeId();
            if (transactionId == null) return;

            var res = await _client.GetOpenPositions(transactionId, true);

            var openPosition = res.Result.FirstOrDefault(x => x.Key == transactionId).Value;
            if (openPosition != null)
            {
                AssertNotDefault(openPosition.Cost);
                AssertNotDefault(openPosition.Fee);
                AssertNotDefault(openPosition.Net);
                AssertNotDefault(openPosition.OrderFlags);
                AssertNotDefault(openPosition.OrderTransactionId);
                AssertNotDefault(openPosition.OrderType);
                AssertNotDefault(openPosition.Volume);
                AssertNotDefault(openPosition.VolumeClosed);
                AssertNotDefault(openPosition.VolumeInQuoteCurrency);
            }
        }

        [Fact]
        public async Task GetLedgersInfo()
        {
            var res = await _client.GetLedgersInfo();

            if (res.Result.Ledgers.Any())
            {
                var ledgerInfo = res.Result.Ledgers.First();
                AssertNotDefault(ledgerInfo.Key);
                AssertLedgerInfo(ledgerInfo.Value);
            }
        }

        [Fact]
        public async Task QueryLedgers()
        {
            string ledgerId = await TryGetLedgerId();
            if (ledgerId == null) return;

            var res = await _client.QueryLedgers(ledgerId);

            var ledgerInfo = res.Result.First(x => x.Key == ledgerId).Value;
            AssertLedgerInfo(ledgerInfo);
        }

        [Fact]
        public async Task GetTradeVolume()
        {
            // "fee-info" param does not seem to be respected on Kraken's side. Not sure if bug ...

            var res = await _client.GetTradeVolume(DefaultPair, includeFeeInfo: false);

            AssertNotDefault(res.Result.Currency);
        }

        [Fact]
        public async Task AddStandardOrder()
        {
            // Minimum volume to trade is 5 USD worth of currency.
            var res = await _client.AddStandardOrder(
                DefaultPair,
                "buy",
                "market",
                volume: 0.1m,
                validate: true);

            AssertNotDefault(res.Result.Description.Order);
        }

        [Fact]
        public async Task CancelOpenOrder_InvalidTransactionId()
        {
            // We only test failing path because testing a success path requires an actual monetary
            // open order to be present.
            try
            {
                await _client.CancelOpenOrder("invalid-order-id");
                Assert.True(false); // Fail.
            }
            catch (KrakenException ex)
            {
                var firstError = ex.Errors.First();
                Assert.Equal(ErrorString.SeverityCodeError, firstError.SeverityCode);
                Assert.Equal(ErrorString.ErrorCategoryOrder, firstError.ErrorCategory);
            }
        }

        private void AssertOrderInfo(OrderInfo orderInfo)
        {
            AssertNotDefault(orderInfo.Description.Pair);
            AssertNotDefault(orderInfo.Description.Type);
            AssertNotDefault(orderInfo.Description.Price);
            AssertNotDefault(orderInfo.Description.Leverage);
            AssertNotDefault(orderInfo.Description.Order);
            AssertNotDefault(orderInfo.OpenTime);
            AssertNotDefault(orderInfo.OrderFlags);
            AssertNotDefault(orderInfo.Status);
        }

        private void AssertLedgerInfo(LedgerInfo ledgerInfo)
        {
            AssertNotDefault(ledgerInfo.Amount);
            AssertNotDefault(ledgerInfo.Asset);
            AssertNotDefault(ledgerInfo.AssetClass);
            AssertNotDefault(ledgerInfo.Fee);
            AssertNotDefault(ledgerInfo.RefId);
            AssertNotDefault(ledgerInfo.Time);
            AssertNotDefault(ledgerInfo.Type);
        }

        private void AssertTradeInfo(TradeInfo tradeInfo)
        {
            AssertNotDefault(tradeInfo.Pair);
            AssertNotDefault(tradeInfo.Time);
            AssertNotDefault(tradeInfo.Type);
            AssertNotDefault(tradeInfo.OrderType);
            AssertNotDefault(tradeInfo.Price);
            AssertNotDefault(tradeInfo.Cost);
            AssertNotDefault(tradeInfo.Fee);
            AssertNotDefault(tradeInfo.Volume);
        }

        private Task<string> TryGetOrderId()
        {
            return TryGetTransactionId(FirstOrderIdOrDefault);

            string FirstOrderIdOrDefault(Dictionary<string, OrderInfo> orders)
                => orders?.FirstOrDefault().Key;
        }

        private Task<string> TryGetTradeId()
        {
            return TryGetTransactionId(FirstTradeIdOrDefault);

            string FirstTradeIdOrDefault(Dictionary<string, OrderInfo> orders)
                => orders?.FirstOrDefault(x => x.Value.Trades?.Any() ?? false).Value?.Trades.First();
        }

        private async Task<string> TryGetTransactionId(Func<Dictionary<string, OrderInfo>, string> selector)
        {
            // We try to get a transaction id from closed orders first. If none found, try from open
            // orders. These must be called sequentially to ensure nonce is received sequentially by
            // the API.

            var closedOrders = await _client.GetClosedOrders(true);
            string result = selector(closedOrders.Result.Closed);

            if (result != null) return result;

            var openOrders = await _client.GetOpenOrders(true);
            return selector(openOrders.Result.Open);
        }

        private async Task<string> TryGetLedgerId()
        {
            var res = await _client.GetLedgersInfo();
            return res.Result.Ledgers.FirstOrDefault().Key;
        }
    }
}