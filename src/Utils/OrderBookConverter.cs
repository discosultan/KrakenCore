using KrakenCore.Models;
using Newtonsoft.Json;
using System;

namespace KrakenCore.Utils
{
    internal class OrderBookConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new OrderBook();

            reader.Read(); // StartObject.
            reader.Read(); // PropertyName of OrderBook.
            reader.Read(); // StartObject.
            reader.Read(); // PropertyName of Asks.
            result.Asks = serializer.Deserialize<Order[]>(reader);
            reader.Read(); // EndArray.
            reader.Read(); // PropertyName of Asks.
            result.Bids = serializer.Deserialize<Order[]>(reader);
            reader.Read(); // EndArray.
            reader.Read(); // EndObject.

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
