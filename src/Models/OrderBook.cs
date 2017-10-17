using KrakenCore.Utils;
using Newtonsoft.Json;

namespace KrakenCore.Models
{
    //[JsonConverter(typeof(OrderBookConverter))]
    public class OrderBook
    {
        /// <summary>
        /// Ask side array of array entries({price}, {volume}, {timestamp}).
        /// </summary>
        public Order[] Asks { get; set; }

        /// <summary>
        /// Bid side array of array entries({price}, {volume}, {timestamp}).
        /// </summary>
        public Order[] Bids { get; set; }
    }

    [JsonConverter(typeof(JArrayToObjectConverter))]
    public class Order
    {
        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public long Timestamp { get; set; }
    }
}
