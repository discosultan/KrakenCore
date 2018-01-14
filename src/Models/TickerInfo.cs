using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class TickerInfo
    {
        /// <summary>
        /// Ask array({price}, {whole lot volume}, {lot volume}).
        /// </summary>
        [JsonProperty("a")]
        public decimal[] Ask;

        /// <summary>
        /// Bid array({price}, {whole lot volume}, {lot volume}).
        /// </summary>
        [JsonProperty("b")]
        public decimal[] Bid;

        /// <summary>
        /// Last trade closed array({price}, {lot volume}).
        /// </summary>
        [JsonProperty("c")]
        public decimal[] Closed;

        /// <summary>
        /// Volume array({today}, {last 24 hours}).
        /// </summary>
        [JsonProperty("v")]
        public decimal[] Volume;

        /// <summary>
        /// Volume weighted average price array({today}, {last 24 hours}).
        /// </summary>
        [JsonProperty("p")]
        public decimal[] Vwap;

        /// <summary>
        /// Number of trades array({today}, {last 24 hours}).
        /// </summary>
        [JsonProperty("t")]
        public int[] Trades;

        /// <summary>
        /// Low array({today}, {last 24 hours}).
        /// </summary>
        [JsonProperty("l")]
        public decimal[] Low;

        /// <summary>
        /// High array({today}, {last 24 hours}).
        /// </summary>
        [JsonProperty("h")]
        public decimal[] High;

        /// <summary>
        /// Today's opening price.
        /// </summary>
        [JsonProperty("o")]
        public decimal Open;
    }
}
