using KrakenCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrakenCore
{
    public partial class KrakenClient
    {
        /// <summary>
        /// Note: this is to aid in approximating the skew time between the server and client.
        /// </summary>
        public Task<KrakenResponse<ServerTime>> GetServerTime()
        {
            return Get<ServerTime>("/0/public/Time");
        }

        /// <param name="info">
        /// Info to retrieve (optional):
        /// <para>info = all info (default)</para>
        /// </param>
        /// <param name="assetClass">
        /// Asset class (optional):
        /// <para>currency (default)</para>
        /// </param>
        /// <param name="assets">
        /// Comma delimited list of assets to get info on. (optional. default = all for given asset class).
        /// </param>
        /// <returns>Dictionary of asset names and their info.</returns>
        public Task<KrakenResponse<Dictionary<string, AssetInfo>>> GetAssetInfo(
            string info = null,
            string assetClass = null,
            string assets = null)
        {
            return Get<Dictionary<string, AssetInfo>> (
                "/0/public/Assets",
                new Dictionary<string, object>(3)
                {
                    ["info"] = info,
                    ["aclass"] = assetClass,
                    ["asset"] = assets
                }
            );
        }

        /// <param name="info">
        /// Info to retrieve (optional):
        /// <para>info = all info (default)</para>
        /// <para>leverage = leverage info</para>
        /// <para>fees = fees schedule</para>
        /// <para>margin = margin info</para>
        /// </param>
        /// <param name="pairs">Comma delimited list of asset pairs to get info on (optional. default = all).</param>
        /// <returns>Dictionary of pair names and their info.</returns>
        public Task<KrakenResponse<Dictionary<string, AssetPair>>> GetTradableAssetPairs(
            string info = null,
            string pairs = null)
        {
            return Get<Dictionary<string, AssetPair>>(
                "/0/public/AssetPairs",
                new Dictionary<string, object>(2)
                {
                    ["info"] = info,
                    ["pair"] = pairs
                }
            );
        }

        /// <summary>Note: today's prices start at 00:00:00 UTC.</summary>
        /// <param name="pairs">Comma delimited list of asset pairs to get info on.</param>
        /// <returns>Dictionary of pair names and their ticker info.</returns>
        public Task<KrakenResponse<Dictionary<string, TickerInformation>>> GetTickerInformation(string pairs)
        {
            return Get<Dictionary<string, TickerInformation>> (
                "/0/public/Ticker",
                new Dictionary<string, object>(1)
                {
                    ["pair"] = pairs
                }
            );
        }

        /// <summary>
        /// Note: the last entry in the OHLC array is for the current, not-yet-committed frame and
        ///       will always be present, regardless of the value of "since".
        /// </summary>
        /// <param name="pair">Asset pair to get OHLC data for.</param>
        /// <param name="interval">
        /// Time frame interval in minutes (optional):
        /// <para>1 (default), 5, 15, 30, 60, 240, 1440, 10080, 21600</para>
        /// </param>
        /// <param name="since">Return committed OHLC data since given id (optional. exclusive).</param>
        /// <returns>Dictionary of pair name and OHLC data</returns>
        public Task<KrakenResponse<OhlcData>> GetOhlcData(string pair, int? interval = null, long? since = null)
        {
            return Get<OhlcData>(
                "/0/public/OHLC",
                new Dictionary<string, object>(3)
                {
                    ["pair"] = pair,
                    ["interval"] = interval,
                    ["since"] = since
                }
            );
        }

        /// <param name="pair">Asset pair to get market depth for.</param>
        /// <param name="count">Maximum number of asks/bids (optional).</param>
        /// <returns>Dictionary of pair name and market depth.</returns>
        public Task<KrakenResponse<Dictionary<string, OrderBook>>> GetOrderBook(string pair, int? count = null)
        {
            return Get<Dictionary<string, OrderBook>>(
                "/0/public/Depth",
                new Dictionary<string, object>(2)
                {
                    ["pair"] = pair,
                    ["count"] = count
                }
            );
        }

        /// <param name="pair">Asset pair to get trade data for.</param>
        /// <param name="since">Return trade data since given id (optional. exclusive).</param>
        /// <returns>Dictionary of pair name and recent trade data.</returns>
        public Task<KrakenResponse<RecentTradesData>> GetRecentTrades(string pair, long? since = null)
        {
            return Get<RecentTradesData>(
                "/0/public/Trades",
                new Dictionary<string, object>(2)
                {
                    ["pair"] = pair,
                    ["since"] = since
                }
            );
        }

        /// <summary>
        /// Note: "since" is inclusive so any returned data with the same time as the previous set
        ///       should overwrite all of the previous set's entries at that time.
        /// </summary>
        /// <param name="pair">Asset pair to get spread data for.</param>
        /// <param name="since">Return spread data since given id (optional. inclusive).</param>
        /// <returns>Dictionary of pair name and recent spread data.</returns>
        public Task<KrakenResponse<SpreadData>> GetRecentSpreadData(string pair, long? since = null)
        {
            return Get<SpreadData>(
                "/0/public/Spread",
                new Dictionary<string, object>(2)
                {
                    ["pair"] = pair,
                    ["since"] = since
                }
            );
        }
    }
}
