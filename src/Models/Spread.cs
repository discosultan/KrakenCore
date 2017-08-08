using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
    [JsonConverter(typeof(TimestampedPairResultConverter<Spread>))]
    public class RecentSpreadData
    {
        public Spread[] Values { get; set; }

        /// <summary>
        /// Id to be used as since when polling for new spread data.
        /// </summary>
        public long Last { get; set; }
    }

    [JsonConverter(typeof(JArrayToObjectConverter))]
    public class Spread
    {
        public long Time { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }
    }
}