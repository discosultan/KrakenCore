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

            Assert.Empty(res.Errors);
            AssertNotDefault(res.Result.UnixTime);
            AssertNotDefault(res.Result.Rfc1123);
        }

        [Fact]
        public async Task GetAssetInfo_All()
        {
            var res = await _client.GetAssetInfo();

            Assert.Empty(res.Errors);
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

            Assert.Empty(res.Errors);
            var firstInfo = res.Result.Values.First();
            Assert.Equal(AssetInfo.AssetClassCurrency, firstInfo.AssetClass);
            Assert.Equal("ETH", firstInfo.AlternateName);
        }

        [Fact]
        public async Task GetAssetInfo_InvalidAssetClass()
        {
            var res = await _client.GetAssetInfo(assetClass: "invalid");

            Assert.NotEmpty(res.Errors);
            var firstError = res.Errors.First();
            Assert.Equal(ErrorString.SeverityCodeError, firstError.SeverityCode);
            Assert.Equal(ErrorString.ErrorCategoryQuery, firstError.ErrorCategory);
        }

        [Fact]
        public async Task GetTradableAssetPairs()
        {
            var res = await _client.GetTradableAssetPairs();

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetTickerInformation()
        {
            var res = await _client.GetTickerInformation("ETHEUR");

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetOhclData()
        {
            var res = await _client.GetOhlcData("ETHEUR");

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetOrderBook()
        {
            var res = await _client.GetOrderBook("ETHEUR");

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetRecentTrades()
        {
            var res = await _client.GetRecentTrades("ETHEUR");

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task GetRecentSpreadData()
        {
            var res = await _client.GetRecentSpreadData("ETHEUR");

            Assert.Empty(res.Errors);
        }
    }
}
