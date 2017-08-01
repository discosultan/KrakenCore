using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace KrakenCore.Models
{
    public class LedgersInfo
    {
        [JsonIgnore]
        private bool _isDirty = true;

        [JsonIgnore]
        private Dictionary<string, LedgerInfo> _ledgers;

        [JsonExtensionData]
        private IDictionary<string, JToken> _extenion;

        public long Count { get; set; }

        [JsonIgnore]
        public Dictionary<string, LedgerInfo> Ledgers
        {
            get
            {
                if (_isDirty)
                {
                    _ledgers = _extenion.ToDictionary(
                        x => x.Key,
                        x => x.Value.ToObject<LedgerInfo>());
                    _isDirty = false;
                }
                return _ledgers;
            }
            set
            {
                _ledgers = value;
                _isDirty = false;
            }
        }
    }

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