using KrakenCore.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KrakenCore.Tests
{
    public class KrakenClientPublicApiTests : KrakenClientTests
    {
        public KrakenClientPublicApiTests(ITestOutputHelper output, KrakenFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact]
        public async Task GetServerTime()
        {
            var res = await Client.GetServerTime();

            AssertNotDefault(res.Result.UnixTime);
            AssertNotDefault(res.Result.Rfc1123);
        }

        [Fact]
        public async Task GetAssetInfo_All()
        {
            var res = await Client.GetAssetInfo();

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
            var res = await Client.GetAssetInfo(assetClass: AssetInfo.AssetClassCurrency, assets: DefaultBase);

            var assetInfo = res.Result.First();
            Assert.Equal(DefaultBase, assetInfo.Key);
            Assert.Equal(AssetInfo.AssetClassCurrency, assetInfo.Value.AssetClass);
            Assert.Equal(DefaultBaseAlternate, assetInfo.Value.AlternateName);
        }

        [Fact]
        public async Task GetAssetInfo_InvalidAssetClass()
        {
            try
            {
                await Client.GetAssetInfo(assetClass: "invalid-asset-class");
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
            var res = await Client.GetTradableAssetPairs();

            var assetPair = res.Result.First(x => x.Key == DefaultPair).Value;
            Assert.Equal(DefaultPairAlternate, assetPair.AlternateName);
            Assert.Equal(AssetInfo.AssetClassCurrency, assetPair.AssetClassBase);
            Assert.Equal(AssetInfo.AssetClassCurrency, assetPair.AssetClassQuote);
            Assert.Equal(DefaultBase, assetPair.Base);
            Assert.Equal(DefaultQuote, assetPair.Quote);
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
            var res = await Client.GetTickerInformation(DefaultPair);

            var tickerInfo = res.Result.First(x => x.Key == DefaultPair).Value;
            AssertNotDefault(tickerInfo.Ask);
            AssertNotDefault(tickerInfo.Bid);
            AssertNotDefault(tickerInfo.Closed);
            AssertNotDefault(tickerInfo.High);
            AssertNotDefault(tickerInfo.Low);
            AssertNotDefault(tickerInfo.Open);
            AssertNotDefault(tickerInfo.Trades);
            AssertNotDefault(tickerInfo.Volume);
            AssertNotDefault(tickerInfo.Vwap);
        }

        [Fact]
        public async Task GetOhlcData()
        {
            var res = await Client.GetOhlcData(DefaultPair);

            AssertNotDefault(res.Result.Last);
            var pair = res.Result.First();
            Assert.Equal(DefaultPair, pair.Key);
            var ohlc = pair.Value.First();
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
            // Note, this can fail if the order book does not have any ask or bid orders for the pair.

            var res = await Client.GetOrderBook(DefaultPair);

            var orderBook = res.Result.First();
            Assert.Equal(DefaultPair, orderBook.Key);
            var ask = orderBook.Value.Asks.First();
            AssertOrderBookEntryNotDefault(ask);
            var bid = orderBook.Value.Bids.First();
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
            var res = await Client.GetRecentTrades(DefaultPair);

            AssertNotDefault(res.Result.Last);
            var trade = res.Result.First(x => x.Key == DefaultPair).Value.First();
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
            var res = await Client.GetRecentSpreadData(DefaultPair);

            AssertNotDefault(res.Result.Last);
            var spread = res.Result.First(x => x.Key == DefaultPair).Value.First();
            AssertNotDefault(spread.Ask);
            AssertNotDefault(spread.Bid);
            AssertNotDefault(spread.Time);
        }
    }
}
