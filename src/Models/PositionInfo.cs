using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class PositionInfo
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
        /// Type of order used to open position (buy/sell).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Order type used to open position.
        /// </summary>
        [JsonProperty("ordertype")]
        public string OrderType { get; set; }

        /// <summary>
        /// Opening cost of position (quote currency unless viqc set in <see cref="OrderFlags"/>).
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Opening fee of position (quote currency).
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Position volume (base currency unless viqc set in oflags).
        /// </summary>
        [JsonProperty("vol")]
        public decimal Volume { get; set; }

        /// <summary>
        /// Position volume closed (base currency unless viqc set in oflags).
        /// </summary>
        [JsonProperty("vol_closed")]
        public decimal VolumeClosed { get; set; }

        /// <summary>
        /// Initial margin (quote currency).
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// Current value of remaining position (if docalcs requested.  quote currency).
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Unrealized profit/loss of remaining position (if docalcs requested.  quote currency, quote currency scale).
        /// </summary>
        public decimal Net { get; set; }

        /// <summary>
        /// Comma delimited list of miscellaneous info.
        /// </summary>
        public string Misc { get; set; }

        /// <summary>
        /// Comma delimited list of order flags.
        /// </summary>
        [JsonProperty("oflags")]
        public string OrderFlags { get; set; }

        /// <summary>
        /// Volume in quote currency.
        /// </summary>
        [JsonProperty("viqc")]
        public decimal VolumeInQuoteCurrency { get; set; }
    }
}
