using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class OrderBook
    {
        /// <summary>
        /// Ask side array of array entries({price}, {volume}, {timestamp}).
        /// </summary>
        public OrderBookEntry[] Asks { get; set; }

        /// <summary>
        /// Bid side array of array entries({price}, {volume}, {timestamp}).
        /// </summary>
        public OrderBookEntry[] Bids { get; set; }
    }

    [JsonConverter(typeof(JsonArrayToStructConverter))]
    public class OrderBookEntry
    {
        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public long Timestamp { get; set; }
    }
}
