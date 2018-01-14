using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class AssetPair
    {
        public const string LotUnit = "unit";

        /// <summary>
        /// Alternate pair name.
        /// </summary>
        [JsonProperty("altname")]
        public string AlternateName { get; set; }

        /// <summary>
        /// Asset class of base component.
        /// </summary>
        [JsonProperty("aclass_base")]
        public string AssetClassBase { get; set; }

        /// <summary>
        /// Asset id of base component.
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Asset class of quote component.
        /// </summary>
        [JsonProperty("aclass_quote")]
        public string AssetClassQuote { get; set; }

        /// <summary>
        /// Asset id of quote component.
        /// </summary>
        public string Quote { get; set; }

        /// <summary>
        /// Volume lot size.
        /// </summary>
        public string Lot { get; set; }

        /// <summary>
        /// Scaling decimal places for pair.
        /// </summary>
        public int PairDecimals { get; set; }

        /// <summary>
        /// Scaling decimal places for volume.
        /// </summary>
        public int LotDecimals { get; set; }

        /// <summary>
        /// Amount to multiply lot volume by to get currency volume.
        /// </summary>
        public int LotMultiplier { get; set; }

        /// <summary>
        /// Array of leverage amounts available when buying.
        /// </summary>
        public decimal[] LeverageBuy { get; set; }

        /// <summary>
        /// Array of leverage amounts available when selling.
        /// </summary>
        public decimal[] LeverageSell { get; set; }

        /// <summary>
        /// Fee schedule array in [volume, percent fee].
        /// </summary>
        public Fee[] Fees { get; set; }

        /// <summary>
        /// Maker fee schedule array in [volume, percent fee] tuples (if on maker/taker).
        /// </summary>
        public Fee[] FeesMaker { get; set; }

        /// <summary>
        /// Volume discount currency.
        /// </summary>
        public string FeeVolumeCurrency { get; set; }

        /// <summary>
        /// Margin call level.
        /// </summary>
        public decimal MarginCall { get; set; }

        /// <summary>
        /// Stop-out/liquidation margin level.
        /// </summary>
        public decimal MarginStop { get; set; }
    }

    [JsonConverter(typeof(JArrayToObjectConverter))]
    public struct Fee
    {
        public decimal Volume;
        public decimal PercentFee;
    }
}
