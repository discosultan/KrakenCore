using KrakenCore.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace KrakenCore
{
    public partial class KrakenClient
    {
        /// <summary>
        /// Note: this is to aid in approximating the skew time between the server and client.
        /// </summary>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<ServerTime>> GetServerTime()
        {
            return QueryPublic<ServerTime>("/0/public/Time");
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
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, AssetInfo>>> GetAssetInfo(
            string info = null,
            string assetClass = null,
            string assets = null)
        {
            return QueryPublic<Dictionary<string, AssetInfo>>(
                "/0/public/Assets",
                new Dictionary<string, string>(3)
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
        /// <param name="pairs">
        /// Comma delimited list of asset pairs to get info on (optional. default = all).
        /// </param>
        /// <returns>Dictionary of pair names and their info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, AssetPair>>> GetTradableAssetPairs(
            string info = null,
            string pairs = null)
        {
            return QueryPublic<Dictionary<string, AssetPair>>(
                "/0/public/AssetPairs",
                new Dictionary<string, string>(2)
                {
                    ["info"] = info,
                    ["pair"] = pairs
                }
            );
        }

        /// <summary>
        /// Note: today's prices start at 00:00:00 UTC.
        /// </summary>
        /// <param name="pairs">Comma delimited list of asset pairs to get info on.</param>
        /// <returns>Dictionary of pair names and their ticker info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, TickerInfo>>> GetTickerInformation(string pairs)
        {
            return QueryPublic<Dictionary<string, TickerInfo>>(
                "/0/public/Ticker",
                new Dictionary<string, string>(1)
                {
                    ["pair"] = pairs
                }
            );
        }

        /// <summary>
        /// Note: the last entry in the OHLC array is for the current, not-yet-committed frame and
        ///       will always be present, regardless of the value of <paramref name="since"/>.
        /// </summary>
        /// <param name="pair">Asset pair to get OHLC data for.</param>
        /// <param name="interval">
        /// Time frame interval in minutes (optional):
        /// <para>1 (default), 5, 15, 30, 60, 240, 1440, 10080, 21600</para>
        /// </param>
        /// <param name="since">Return committed OHLC data since given id (optional. exclusive).</param>
        /// <returns>Dictionary of pair name and OHLC data</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<TimestampedDictionary<string, Ohlc[]>>> GetOhlcData(string pair, int? interval = null, long? since = null)
        {
            return QueryPublic<TimestampedDictionary<string, Ohlc[]>>(
                "/0/public/OHLC",
                new Dictionary<string, string>(3)
                {
                    ["pair"] = pair,
                    ["interval"] = interval?.ToString(Culture),
                    ["since"] = since?.ToString(Culture)
                }
            );
        }

        /// <param name="pair">Asset pair to get market depth for.</param>
        /// <param name="count">Maximum number of asks/bids (optional).</param>
        /// <returns>Dictionary of pair name and market depth.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, OrderBook>>> GetOrderBook(string pair, int? count = null)
        {
            return QueryPublic<Dictionary<string, OrderBook>>(
                "/0/public/Depth",
                new Dictionary<string, string>(2)
                {
                    ["pair"] = pair,
                    ["count"] = count?.ToString(Culture)
                }
            );
        }

        /// <param name="pair">Asset pair to get trade data for.</param>
        /// <param name="since">Return trade data since given id (optional. exclusive).</param>
        /// <returns>Dictionary of pair name and recent trade data.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<TimestampedDictionary<string, Trade[]>>> GetRecentTrades(string pair, long? since = null)
        {
            return QueryPublic<TimestampedDictionary<string, Trade[]>>(
                "/0/public/Trades",
                new Dictionary<string, string>(2)
                {
                    ["pair"] = pair,
                    ["since"] = since?.ToString(Culture)
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
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<TimestampedDictionary<string, Spread[]>>> GetRecentSpreadData(string pair, long? since = null)
        {
            return QueryPublic<TimestampedDictionary<string, Spread[]>>(
                "/0/public/Spread",
                new Dictionary<string, string>(2)
                {
                    ["pair"] = pair,
                    ["since"] = since?.ToString(Culture)
                }
            );
        }
    }
}
