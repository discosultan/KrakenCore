using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
    [JsonConverter(typeof(JArrayToObjectConverter))]
    public class Spread
    {
        public long Time { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }
    }
}
