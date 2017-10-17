using Newtonsoft.Json;
using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class TradeVolume
    {
        /// <summary>
        /// Volume currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Current discount volume.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Fee tier info (if requested).
        /// </summary>
        public Dictionary<string, FeeInfo> Fees { get; set; }

        /// <summary>
        /// Maker fee tier info (if requested) for any pairs on maker/taker schedule.
        /// </summary>
        public Dictionary<string, FeeInfo> FeesMaker { get; set; }
    }

    public class FeeInfo
    {
        /// <summary>
        /// Current fee in percent.
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Minimum fee for pair (if not fixed fee).
        /// </summary>
        [JsonProperty("minfee")]
        public decimal MinFee { get; set; }

        /// <summary>
        /// Maximum fee for pair (if not fixed fee).
        /// </summary>
        [JsonProperty("maxfee")]
        public decimal MaxFee { get; set; }

        /// <summary>
        /// Next tier's fee for pair (if not fixed fee. 0 if at lowest fee tier).
        /// </summary>
        [JsonProperty("nextfee")]
        public decimal NextFee { get; set; }

        /// <summary>
        /// Volume level of next tier (if not fixed fee. 0 if at lowest fee tier).
        /// </summary>
        [JsonProperty("nextvolume")]
        public decimal NextVolume { get; set; }

        /// <summary>
        /// Volume level of current tier (if not fixed fee. 0 if at lowest fee tier).
        /// </summary>
        [JsonProperty("tiervolume")]
        public decimal TierVolume { get; set; }
    }
}
