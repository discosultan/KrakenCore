using Newtonsoft.Json;
using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class TradesHistory
    {
        public Dictionary<string, TradeInfo> Trades { get; set; }

        public int Count { get; set; }
    }

    public class TradeInfo
    {
        /// <summary>
        /// Order responsible for execution of trade.
        /// </summary>
        [JsonProperty("ordertxid")]
        public string OrderTransactionId { get; set; }

        /// <summary>
        /// Asset pair.
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Unix timestamp of trade.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Type of order (buy/sell).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Order type.
        /// </summary>
        [JsonProperty("ordertype")]
        public string OrderType { get; set; }

        /// <summary>
        /// Average price order was executed at (quote currency).
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total cost of order (quote currency).
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Total fee (quote currency).
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Volume (base currency).
        /// </summary>
        [JsonProperty("vol")]
        public decimal Volume { get; set; }

        /// <summary>
        /// Initial margin (quote currency).
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// Comma delimited list of miscellaneous info.
        /// <para>closing = trade closes all or part of a position</para>
        /// </summary>
        public string Misc { get; set; }

        /// <summary>
        /// Position status (open/closed).
        /// </summary>
        [JsonProperty("posstatus")]
        public string PositionStatus { get; set; }

        /// <summary>
        /// Average price of closed portion of position (quote currency).
        /// </summary>
        [JsonProperty("cprice")]
        public decimal? ClosedPrice { get; set; }

        /// <summary>
        /// Total cost of closed portion of position (quote currency).
        /// </summary>
        [JsonProperty("ccost")]
        public decimal? ClosedCost { get; set; }

        /// <summary>
        /// Total fee of closed portion of position (quote currency).
        /// </summary>
        [JsonProperty("cfee")]
        public decimal? ClosedFee { get; set; }

        /// <summary>
        /// Total volume of closed portion of position (quote currency).
        /// </summary>
        [JsonProperty("cvol")]
        public decimal? ClosedVol { get; set; }

        /// <summary>
        /// Total margin freed in closed portion of position (quote currency).
        /// </summary>
        [JsonProperty("cmargin")]
        public decimal? ClosedMargin { get; set; }

        /// <summary>
        /// Net profit/loss of closed portion of position (quote currency, quote currency scale).
        /// </summary>
        public decimal? Net { get; set; }

        /// <summary>
        /// List of closing trades for position (if available).
        /// </summary>
        public string[] Trades { get; set; }
    }
}
