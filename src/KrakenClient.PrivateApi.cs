using KrakenCore.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace KrakenCore
{
    public partial class KrakenClient
    {
        /// <returns>Dictionary of asset names and balance amount.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, decimal>>> GetAccountBalance()
        {
            return QueryPrivate<Dictionary<string, decimal>>("/0/private/Balance");
        }

        /// <summary>
        /// Note: rates used for the floating valuation is the midpoint of the best bid and ask prices.
        /// </summary>
        /// <param name="assetClass">
        /// Asset class (optional):
        /// <para>currency (default)</para>
        /// </param>
        /// <param name="asset">Base asset used to determine balance (default = ZUSD).</param>
        /// <returns>Array of trade balance info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<TradeBalanceInfo>> GetTradeBalance(string assetClass = null, string asset = "ZUSD")
        {
            return QueryPrivate<TradeBalanceInfo>(
                "/0/private/TradeBalance",
                new Dictionary<string, string>(2 + AdditionalPrivateQueryArgs)
                {
                    ["aclass"] = assetClass,
                    ["asset"] = asset
                }
            );
        }

        /// <summary>
        /// Note: Unless otherwise stated, costs, fees, prices, and volumes are in the asset pair's
        ///       scale, not the currency's scale. For example, if the asset pair uses a lot size
        ///       that has a scale of 8, the volume will use a scale of 8, even if the currency it
        ///       represents only has a scale of 2. Similarly, if the asset pair's pricing scale is
        ///       5, the scale will remain as 5, even if the underlying currency has a scale of 8.
        /// </summary>
        /// <param name="includeTrades">
        /// Whether or not to include trades in output (optional. default = false).
        /// </param>
        /// <param name="userRef">Restrict results to given user reference id (optional).</param>
        /// <returns>Dictionary of order info in open array with txid as the key.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<OpenOrders>> GetOpenOrders(bool includeTrades = false, string userRef = null)
        {
            return QueryPrivate<OpenOrders>(
                "/0/private/OpenOrders",
                new Dictionary<string, string>(2 + AdditionalPrivateQueryArgs)
                {
                    ["trades"] = includeTrades ? "true" : null,
                    ["userref"] = userRef
                }
            );
        }

        /// <summary>
        /// Note: Times given by order tx ids are more accurate than unix timestamps. If an order tx
        ///       id is given for the time, the order's open time is used.
        /// </summary>
        /// <param name="includeTrades">
        /// Whether or not to include trades in output (optional. default = false).
        /// </param>
        /// <param name="userRef">Restrict results to given user reference id (optional).</param>
        /// <param name="start">Starting unix timestamp or order tx id of results (optional. exclusive).</param>
        /// <param name="end">Ending unix timestamp or order tx id of results (optional. inclusive).</param>
        /// <param name="offset">Result offset.</param>
        /// <param name="closeTime">
        /// Which time to use (optional).
        /// <para>open</para>
        /// <para>close</para>
        /// <para>both (default)</para>
        /// </param>
        /// <returns>Array of order info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<ClosedOrders>> GetClosedOrders(
            bool includeTrades = false,
            string userRef = null,
            long? start = null,
            long? end = null,
            int? offset = null,
            string closeTime = null)
        {
            return QueryPrivate<ClosedOrders>(
                "/0/private/ClosedOrders",
                new Dictionary<string, string>(6 + AdditionalPrivateQueryArgs)
                {
                    ["trades"] = includeTrades ? "true" : null,
                    ["userref"] = userRef,
                    ["start"] = start?.ToString(Culture),
                    ["end"] = end?.ToString(Culture),
                    ["ofs"] = offset?.ToString(Culture),
                    ["closetime"] = closeTime
                }
            );
        }

        /// <param name="transactionIds">
        /// Comma delimited list of transaction ids to query info about (20 maximum).
        /// </param>
        /// <param name="includeTrades">
        /// Whether or not to include trades in output (optional. default = false).
        /// </param>
        /// <param name="userRef">Restrict results to given user reference id (optional).</param>
        /// <returns>Dictionary of orders info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, OrderInfo>>> QueryOrdersInfo(
            string transactionIds,
            bool includeTrades = false,
            string userRef = null)
        {
            return QueryPrivate<Dictionary<string, OrderInfo>>(
                "/0/private/QueryOrders",
                new Dictionary<string, string>(3 + AdditionalPrivateQueryArgs)
                {
                    ["trades"] = includeTrades ? "true" : null,
                    ["userref"] = userRef,
                    ["txid"] = transactionIds
                }
            );
        }

        /// <summary>
        /// Note:
        /// <para>
        /// Unless otherwise stated, costs, fees, prices, and volumes are in the asset pair's scale,
        /// not the currency's scale.
        /// </para>
        /// <para>Times given by trade tx ids are more accurate than unix timestamps.</para>
        /// </summary>
        /// <param name="type">
        /// Type of trade (optional):
        /// <para>all = all types (default)</para>
        /// <para>any position = any position (open or closed)</para>
        /// <para>closed position = positions that have been closed</para>
        /// <para>closing position = any trade closing all or part of a position</para>
        /// <para>no position = non-positional trades</para>
        /// </param>
        /// <param name="includeTrades">
        /// Whether or not to include trades related to position in output (optional. default = false).
        /// </param>
        /// <param name="start">Starting unix timestamp or trade tx id of results (optional. exclusive).</param>
        /// <param name="end">Ending unix timestamp or trade tx id of results (optional. inclusive).</param>
        /// <param name="offset">Result offset.</param>
        /// <returns>Dictionary of trade info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<TradesHistory>> GetTradesHistory(
            string type = null,
            bool includeTrades = false,
            long? start = null,
            long? end = null,
            int? offset = null)
        {
            return QueryPrivate<TradesHistory>(
                "/0/private/TradesHistory",
                new Dictionary<string, string>(5 + AdditionalPrivateQueryArgs)
                {
                    ["type"] = type,
                    ["trades"] = includeTrades ? "true" : null,
                    ["start"] = start?.ToString(Culture),
                    ["end"] = end?.ToString(Culture),
                    ["ofs"] = offset?.ToString(Culture)
                },
                2
            );
        }

        /// <param name="transactionIds">
        /// Comma delimited list of transaction ids to query info about (20 maximum).
        /// </param>
        /// <param name="includeTrades">
        /// Whether or not to include trades related to position in output (optional. default = false).
        /// </param>
        /// <returns>Dictionary of trades info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, TradeInfo>>> QueryTradesInfo(
            string transactionIds,
            bool includeTrades = false)
        {
            return QueryPrivate<Dictionary<string, TradeInfo>>(
                "/0/private/QueryTrades",
                new Dictionary<string, string>(2 + AdditionalPrivateQueryArgs)
                {
                    ["trades"] = includeTrades ? "true" : null,
                    ["txid"] = transactionIds
                }
            );
        }

        /// <summary>
        /// Note: Unless otherwise stated, costs, fees, prices, and volumes are in the asset pair's
        ///       scale, not the currency's scale.
        /// </summary>
        /// <param name="transactionIds">
        /// Comma delimited list of transaction ids to restrict output to.
        /// </param>
        /// <param name="doCalculations">
        /// Whether or not to include profit/loss calculations (optional. default = false).
        /// </param>
        /// <returns>Dictionary of open position info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, PositionInfo>>> GetOpenPositions(
            string transactionIds,
            bool doCalculations = false)
        {
            return QueryPrivate<Dictionary<string, PositionInfo>>(
                "/0/private/OpenPositions",
                new Dictionary<string, string>(2 + AdditionalPrivateQueryArgs)
                {
                    ["txid"] = transactionIds,
                    ["docalcs"] = doCalculations ? "true" : null
                }
            );
        }

        /// <summary>
        /// Note: Times given by ledger ids are more accurate than unix timestamps.
        /// </summary>
        /// <param name="assetClass">
        /// Asset class (optional):
        /// <para>currency (default)</para>
        /// </param>
        /// <param name="assets">
        /// Comma delimited list of assets to restrict output to (optional. default = all).
        /// </param>
        /// <param name="type">
        /// Type of ledger to retrieve (optional):
        /// <para>all (default)</para>
        /// <para>deposit</para>
        /// <para>withdrawal</para>
        /// <para>trade</para>
        /// <para>margin</para>
        /// </param>
        /// <param name="start">Starting unix timestamp or ledger id of results (optional. exclusive).</param>
        /// <param name="end">Ending unix timestamp or ledger id of results (optional. inclusive).</param>
        /// <param name="offset">Result offset.</param>
        /// <returns>Dictionary of ledgers info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<LedgersInfo>> GetLedgersInfo(
            string assetClass = null,
            string assets = null,
            string type = null,
            string start = null,
            string end = null,
            string offset = null)
        {
            return QueryPrivate<LedgersInfo>(
                "/0/private/Ledgers",
                new Dictionary<string, string>(6 + AdditionalPrivateQueryArgs)
                {
                    ["aclass"] = assetClass,
                    ["asset"] = assets,
                    ["type"] = type,
                    ["start"] = start,
                    ["end"] = end,
                    ["ofs"] = offset
                },
                2
            );
        }

        /// <param name="ids">Comma delimited list of ledger ids to query info about (20 maximum).</param>
        /// <returns>Dictionary of ledgers info.</returns>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<Dictionary<string, LedgerInfo>>> QueryLedgers(string ids)
        {
            return QueryPrivate<Dictionary<string, LedgerInfo>>(
                "/0/private/QueryLedgers",
                new Dictionary<string, string>(1 + AdditionalPrivateQueryArgs)
                {
                    ["id"] = ids
                }
            );
        }

        /// <summary>
        /// Note: If an asset pair is on a maker/taker fee schedule, the taker side is given in
        ///       "fees" and maker side in "fees_maker". For pairs not on maker/taker, they will only
        ///       be given in "fees".
        /// </summary>
        /// <param name="pair">Comma delimited list of asset pairs to get fee info on (optional).</param>
        /// <param name="includeFeeInfo">Whether or not to include fee info in results (optional).</param>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<TradeVolume>> GetTradeVolume(string pair = null, bool includeFeeInfo = false)
        {
            return QueryPrivate<TradeVolume>(
                "/0/private/TradeVolume",
                new Dictionary<string, string>(2 + AdditionalPrivateQueryArgs)
                {
                    ["pair"] = pair,
                    ["fee-info"] = includeFeeInfo ? "true" : null
                }
            );
        }

        /// <summary>
        /// Note:
        /// <para>
        /// See <a href="https://www.kraken.com/help/api#get-tradable-pairs">Get tradable asset
        /// pairs</a> for specifications on asset pair prices, lots, and leverage.
        /// </para>
        /// <para>
        /// Prices can be preceded by +, -, or # to signify the price as a relative amount (with the
        /// exception of trailing stops, which are always relative). + adds the amount to the current
        /// offered price. - subtracts the amount from the current offered price. # will either add
        /// or subtract the amount to the current offered price, depending on the type and order type
        /// used. Relative prices can be suffixed with a % to signify the relative amount as a
        /// percentage of the offered price.
        /// </para>
        /// <para>
        /// For orders using leverage, 0 can be used for the volume to auto-fill the volume needed to
        /// close out your position.
        /// </para>
        /// <para>
        /// If you receive the error "EOrder:Trading agreement required", refer to your API key
        /// management page for further details.
        /// </para>
        /// </summary>
        /// <param name="pair">Asset pair.</param>
        /// <param name="type">Type of order (buy/sell).</param>
        /// <param name="orderType">
        /// Order type:
        /// <para>market</para>
        /// <para>limit (price = limit price)</para>
        /// <para>stop-loss (price = stop loss price)</para>
        /// <para>take-profit (price = take profit price)</para>
        /// <para>stop-loss-profit (price = stop loss price, price2 = take profit price)</para>
        /// <para>stop-loss-profit-limit (price = stop loss price, price2 = take profit price)</para>
        /// <para>stop-loss-limit (price = stop loss trigger price, price2 = triggered limit price)</para>
        /// <para>
        /// take-profit-limit (price = take profit trigger price, price2 = triggered limit price)
        /// </para>
        /// <para>trailing-stop (price = trailing stop offset)</para>
        /// <para>trailing-stop-limit (price = trailing stop offset, price2 = triggered limit offset)</para>
        /// <para>stop-loss-and-limit (price = stop loss price, price2 = limit price)</para>
        /// <para>settle-position</para>
        /// </param>
        /// <param name="volume">Order volume in lots.</param>
        /// <param name="price">Price (optional. dependent upon <paramref name="orderType"/>).</param>
        /// <param name="price2">Secondary price (optional. dependent upon <paramref name="orderType"/>).</param>
        /// <param name="leverage">Amount of leverage desired (optional. default = none).</param>
        /// <param name="orderFlags">
        /// Comma delimited list of order flags (optional):
        /// <para>viqc = volume in quote currency (not available for leveraged orders)</para>
        /// <para>fcib = prefer fee in base currency</para>
        /// <para>fciq = prefer fee in quote currency</para>
        /// <para>nompp = no market price protection</para>
        /// <para>post = post only order (available when <paramref name="orderType"/> = limit)</para>
        /// </param>
        /// <param name="startTime">
        /// Scheduled start time (optional):
        /// <para>0 = now (default)</para>
        /// <para>+{n} = schedule start time {n} seconds from now</para>
        /// <para>{n} = unix timestamp of start time</para>
        /// </param>
        /// <param name="expireTime">
        /// Expiration time (optional):
        /// <para>0 = no expiration (default)</para>
        /// <para>+{n} = expire {n} seconds from now</para>
        /// <para>{n} = unix timestamp of expiration time</para>
        /// </param>
        /// <param name="userRef">User reference id. 32-bit signed number. (optional).</param>
        /// <param name="validate">Validate inputs only. Do not submit order (optional).</param>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<AddOrderResult>> AddStandardOrder(
            string pair,
            string type,
            string orderType,
            decimal volume,
            decimal? price = null,
            decimal? price2 = null,
            string leverage = null,
            string orderFlags = null,
            string startTime = null,
            string expireTime = null,
            string userRef = null,
            bool validate = false)
        {
            return QueryPrivate<AddOrderResult>(
                "/0/private/AddOrder",
                new Dictionary<string, string>(12 + AdditionalPrivateQueryArgs)
                {
                    ["pair"] = pair,
                    ["type"] = type,
                    ["ordertype"] = orderType,
                    ["price"] = price?.ToString(Culture),
                    ["price2"] = price2?.ToString(Culture),
                    ["volume"] = volume.ToString(Culture),
                    ["leverage"] = leverage,
                    ["oflags"] = orderFlags,
                    ["starttm"] = startTime,
                    ["expiretm"] = expireTime,
                    ["userref"] = userRef,
                    ["validate"] = validate ? "true" : null
                },
                0
            );
        }

        /// <summary>
        /// Note: <paramref name="transactionId"/> may be a user reference id.
        /// </summary>
        /// <param name="transactionId">Transaction id.</param>
        /// <exception cref="HttpRequestException">There was a problem with the HTTP request.</exception>
        /// <exception cref="KrakenException">There was a problem with the Kraken API call.</exception>
        public Task<KrakenResponse<CancelOrderResult>> CancelOpenOrder(string transactionId)
        {
            return QueryPrivate<CancelOrderResult>(
                "/0/private/CancelOrder",
                new Dictionary<string, string>(1 + AdditionalPrivateQueryArgs)
                {
                    ["txid"] = transactionId
                },
                0
            );
        }
    }
}
