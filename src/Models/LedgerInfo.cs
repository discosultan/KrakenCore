using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class LedgerInfo
    {
        /// <summary>
        /// Reference id.
        /// </summary>
        [JsonProperty("refid")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Unix timestamp of ledger.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Type of ledger entry.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Asset class.
        /// </summary>
        [JsonProperty("aclass")]
        public string AssetClass { get; set; }

        /// <summary>
        /// Asset.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Transaction amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction fee.
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Resulting balance.
        /// </summary>
        public decimal Balance { get; set; }
    }
}