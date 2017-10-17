using Newtonsoft.Json;
using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class OpenOrders
    {
        public Dictionary<string, OrderInfo> Open { get; set; }
    }

    public class ClosedOrders
    {
        public Dictionary<string, OrderInfo> Closed { get; set; }

        public int Count { get; set; }
    }

    public class OrderInfo
    {
        public const string StatusOpen = "open";
        public const string StatusClosed = "closed";

        /// <summary>
        /// Referral order transaction id that created this order.
        /// </summary>
        [JsonProperty("refid")]
        public string RefId { get; set; }

        /// <summary>
        /// User reference id.
        /// </summary>
        [JsonProperty("userref")]
        public int? UserRef { get; set; }

        /// <summary>
        /// Status of order.
        /// <para>pending = order pending book entry</para>
        /// <para>open = open order</para>
        /// <para>closed = closed order</para>
        /// <para>canceled = order cancelled</para>
        /// <para>expired = order expired</para>
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Unix timestamp of when order was placed.
        /// </summary>
        [JsonProperty("opentm")]
        public double OpenTime { get; set; }

        /// <summary>
        /// Unix timestamp of order start time (or 0 if not set).
        /// </summary>
        [JsonProperty("starttm")]
        public double StartTime { get; set; }

        /// <summary>
        /// Unix timestamp of order end time (or 0 if not set).
        /// </summary>
        [JsonProperty("expiretm")]
        public double ExpireTime { get; set; }

        /// <summary>
        /// Unix timestamp of when order was closed.
        /// </summary>
        [JsonProperty("closetm")]
        public double CloseTime { get; set; }

        /// <summary>
        /// Additional info on status (if any).
        /// </summary>
        public string Reason { get; set; } // Nullable

        /// <summary>
        /// Order description info.
        /// </summary>
        [JsonProperty("descr")]
        public OrderDescription Description { get; set; }

        /// <summary>
        /// Volume of order (base currency unless viqc set in <see cref="OrderFlags"/>).
        /// </summary>
        [JsonProperty("vol")]
        public decimal Volume { get; set; }

        /// <summary>
        /// Volume executed (base currency unless viqc set in <see cref="OrderFlags"/>).
        /// </summary>
        [JsonProperty("vol_exec")]
        public decimal VolumeExecuted { get; set; }

        /// <summary>
        /// Total cost (quote currency unless unless viqc set in <see cref="OrderFlags"/>).
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Total fee (quote currency).
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Average price (quote currency unless viqc set in <see cref="OrderFlags"/>).
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Stop price (quote currency, for trailing stops)
        /// </summary>
        [JsonProperty("stopprice")]
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// Triggered limit price (quote currency, when limit based order type triggered).
        /// </summary>
        [JsonProperty("limitprice")]
        public decimal? LimitPrice { get; set; }

        /// <summary>
        /// Comma delimited list of miscellaneous info.
        /// <para>stopped = triggered by stop price</para>
        /// <para>touched = triggered by touch price</para>
        /// <para>liquidated = liquidation</para>
        /// <para>partial = partial fill</para>
        /// </summary>
        public string Misc { get; set; }

        /// <summary>
        /// Comma delimited list of order flags.
        /// <para>viqc = volume in quote currency</para>
        /// <para>fcib = prefer fee in base currency (default if selling)</para>
        /// <para>fciq = prefer fee in quote currency (default if buying)</para>
        /// <para>nompp = no market price protection</para>
        /// </summary>
        [JsonProperty("oflags")]
        public string OrderFlags { get; set; }

        /// <summary>
        /// Array of trade ids related to order (if trades info requested and data available).
        /// </summary>
        public string[] Trades { get; set; }
    }

    public class OrderDescription
    {
        public const string TypeSell = "sell";
        public const string TypeBuy = "buy";

        /// <summary>
        /// Asset pair.
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Type of order (buy/sell).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Order type (See <see cref="KrakenClient.AddStandardOrder"/>).
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Primary price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Secondary price
        /// </summary>
        public decimal Price2 { get; set; }

        /// <summary>
        /// Amount of leverage
        /// </summary>
        public string Leverage { get; set; }

        /// <summary>
        /// Order description.
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// Conditional close order description (if conditional close set).
        /// </summary>
        public string Close { get; set; }
    }
}
