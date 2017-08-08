using Newtonsoft.Json;
using System;
using System.Reflection;

namespace KrakenCore.Utils
{
    internal class TimestampedPairResultConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = existingValue ?? Activator.CreateInstance(objectType);

            reader.Read(); // StartObject.
            reader.Read(); // PropertyName of pair.
            objectType.GetRuntimeProperty("Values").SetValue(result, serializer.Deserialize<T[]>(reader));
            reader.Read(); // EndArray.
            reader.Read(); // PropertyName of Last.
            objectType.GetRuntimeProperty("Last").SetValue(result, serializer.Deserialize<long>(reader));
            reader.Read(); // EndObject.

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}