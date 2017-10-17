using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class TradeBalanceInfo
    {
        /// <summary>
        /// Equivalent balance (combined balance of all currencies).
        /// </summary>
        [JsonProperty(PropertyName = "eb")]
        public decimal EquivalentBalance { get; set; }

        /// <summary>
        /// Trade balance (combined balance of all equity currencies).
        /// </summary>
        [JsonProperty(PropertyName = "tb")]
        public decimal TradeBalance { get; set; }

        /// <summary>
        /// Margin amount of open positions.
        /// </summary>
        [JsonProperty(PropertyName = "m")]
        public decimal MarginAmount { get; set; }

        /// <summary>
        /// Unrealized net profit/loss of open positions.
        /// </summary>
        [JsonProperty(PropertyName = "n")]
        public decimal UnrealizedProfitAndLoss { get; set; }

        /// <summary>
        /// Cost basis of open positions.
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public decimal CostBasis { get; set; }

        /// <summary>
        /// Current floating valuation of open positions.
        /// </summary>
        [JsonProperty(PropertyName = "v")]
        public decimal FloatingValuation { get; set; }

        /// <summary>
        /// Equity = trade balance + unrealized net profit/loss.
        /// </summary>
        [JsonProperty(PropertyName = "e")]
        public decimal Equity { get; set; }

        /// <summary>
        /// Free margin = equity - initial margin (maximum margin available to open new positions).
        /// </summary>
        [JsonProperty(PropertyName = "mf")]
        public decimal FreeMargin { get; set; }

        /// <summary>
        /// Margin level = (equity / initial margin) * 100.
        /// </summary>
        [JsonProperty(PropertyName = "ml")]
        public decimal MarginLevel { get; set; }
    }
}
