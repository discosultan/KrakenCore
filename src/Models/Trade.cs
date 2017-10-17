using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
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
