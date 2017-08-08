using KrakenCore.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests
{
    public partial class KrakenClientTests
    {
        [Fact]
        public async Task GetServerTime()
        {
            var res = await _client.GetServerTime();

            AssertNotDefault(res.Result.UnixTime);
            AssertNotDefault(res.Result.Rfc1123);
        }

        [Fact]
        public async Task GetAssetInfo_All()
        {
            var res = await _client.GetAssetInfo();

            Assert.NotEmpty(res.Result);
            var firstInfo = res.Result.Values.First();
            AssertNotDefault(firstInfo.AlternateName);
            AssertNotDefault(firstInfo.AssetClass);
            AssertNotDefault(firstInfo.Decimals);
            AssertNotDefault(firstInfo.DisplayDecimals);
        }

        [Fact]
        public async Task GetAssetInfo_Eth()
        {
            var res = await _client.GetAssetInfo(assetClass: AssetInfo.AssetClassCurrency, assets: "ETH");

            var firstInfo = res.Result.Values.First();
            Assert.Equal(AssetInfo.AssetClassCurrency, firstInfo.AssetClass);
            Assert.Equal("ETH", firstInfo.AlternateName);
        }

        [Fact]
        public async Task GetAssetInfo_InvalidAssetClass()
        {
            try
            {
                await _client.GetAssetInfo(assetClass: "invalid");
                Assert.True(false); // Fail.
            }
            catch (KrakenException ex)
            {
                var firstError = ex.Errors.First();
                Assert.Equal(ErrorString.SeverityCodeError, firstError.SeverityCode);
                Assert.Equal(ErrorString.ErrorCategoryQuery, firstError.ErrorCategory);
            }
        }

        [Fact]
        public async Task GetTradableAssetPairs()
        {
            var res = await _client.GetTradableAssetPairs();

            var assetPair = res.Result.First(x => x.Key == "XETHZEUR").Value;
            Assert.Equal("ETHEUR", assetPair.AlternateName);
            Assert.Equal(AssetInfo.AssetClassCurrency, assetPair.AssetClassBase);
            Assert.Equal(AssetInfo.AssetClassCurrency, assetPair.AssetClassQuote);
            Assert.Equal("XETH", assetPair.Base);
            Assert.Equal("ZEUR", assetPair.Quote);
            Assert.Equal(AssetPair.LotUnit, assetPair.Lot);
            AssertNotDefault(assetPair.PairDecimals);
            AssertNotDefault(assetPair.LotDecimals);
            AssertNotDefault(assetPair.LotMultiplier);
            AssertNotDefault(assetPair.LeverageBuy);
            AssertNotDefault(assetPair.LeverageSell);
            AssertNotDefault(assetPair.Fees);
            AssertNotDefault(assetPair.FeesMaker);
        }

        [Fact]
        public async Task GetTickerInformation()
        {
            var res = await _client.GetTickerInformation("ETHEUR");

            var tickerInfo = res.Result.First(x => x.Key == "XETHZEUR").Value;
            AssertNotDefault(tickerInfo.Ask);
            AssertNotDefault(tickerInfo.Bid);
            AssertNotDefault(tickerInfo.Closed);
            AssertNotDefault(tickerInfo.High);
            AssertNotDefault(tickerInfo.Low);
            AssertNotDefault(tickerInfo.Open);
            AssertNotDefault(tickerInfo.Trades);
            AssertNotDefault(tickerInfo.Volume);
            AssertNotDefault(tickerInfo.VWAP);
        }

        [Fact]
        public async Task GetOhlcData()
        {
            var res = await _client.GetOhlcData("ETHEUR");

            AssertNotDefault(res.Result.Last);
            var ohlc = res.Result.Values.First();
            AssertNotDefault(ohlc.Close);
            AssertNotDefault(ohlc.Count);
            AssertNotDefault(ohlc.High);
            AssertNotDefault(ohlc.Low);
            AssertNotDefault(ohlc.Open);
            AssertNotDefault(ohlc.Time);
            AssertNotDefault(ohlc.Volume);
            AssertNotDefault(ohlc.Vwap);
        }

        [Fact]
        public async Task GetOrderBook()
        {
            var res = await _client.GetOrderBook("ETHEUR");

            var ask = res.Result.Asks.First();
            AssertOrderBookEntryNotDefault(ask);
            var bid = res.Result.Bids.First();
            AssertOrderBookEntryNotDefault(bid);

            void AssertOrderBookEntryNotDefault(Order order)
            {
                AssertNotDefault(order.Price);
                AssertNotDefault(order.Timestamp);
                AssertNotDefault(order.Volume);
            }
        }

        [Fact]
        public async Task GetRecentTrades()
        {
            var res = await _client.GetRecentTrades("ETHEUR");

            AssertNotDefault(res.Result.Last);
            var trade = res.Result.Values.First();
            AssertNotDefault(trade.Misc);
            AssertNotDefault(trade.Price);
            AssertNotDefault(trade.Side);
            AssertNotDefault(trade.Time);
            AssertNotDefault(trade.Type);
            AssertNotDefault(trade.Volume);
        }

        [Fact]
        public async Task GetRecentSpreadData()
        {
            var res = await _client.GetRecentSpreadData("ETHEUR");

            AssertNotDefault(res.Result.Last);
            var spread = res.Result.Values.First();
            AssertNotDefault(spread.Ask);
            AssertNotDefault(spread.Bid);
            AssertNotDefault(spread.Time);
        }
    }
}