using KrakenCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KrakenCore.Utils
{
    internal class OpenClosedOrdersConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = Activator.CreateInstance(objectType) as IDictionary<string, OrderInfo>;

            reader.Read(); // StartObject.
            reader.Read(); // PropertyName of open/closed.
            reader.Read(); // StartObject.
            while (reader.TokenType != JsonToken.EndObject)
            {
                var key = reader.Value.ToString();
                reader.Read(); // PropertyName of order.
                var value = serializer.Deserialize<OrderInfo>(reader);
                result.Add(key, value);
                reader.Read(); // EndObject.
            }
            reader.Read(); // EndObject.
            // Skip count if present.
            if (reader.TokenType == JsonToken.PropertyName)
            {
                reader.Read(); // PropertyName.
                reader.Read();
            }

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
