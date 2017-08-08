using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
    [JsonConverter(typeof(TimestampedPairResultConverter<Trade>))]
    public class RecentTrades
    {
        public Trade[] Values { get; set; }

        /// <summary>
        /// Id to be used as since when polling for new trade data.
        /// </summary>
        public long Last { get; set; }
    }

    [JsonConverter(typeof(JArrayToObjectConverter))]
    public class Trade
    {
        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public double Time { get; set; }

        public string Side { get; set; }

        public string Type { get; set; }

        public string Misc { get; set; }
    }
}