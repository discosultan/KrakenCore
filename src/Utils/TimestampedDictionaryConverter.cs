using Newtonsoft.Json;
using System;
using System.Reflection;

namespace KrakenCore.Utils
{
    internal class TimestampedDictionaryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = Activator.CreateInstance(objectType);

            Type typeKey = objectType.GenericTypeArguments[0];
            Type typeValue = objectType.GenericTypeArguments[1];

            var propLast = objectType.GetRuntimeProperty(nameof(TimestampedDictionary<int, int>.Last));
            var methodAdd = objectType.GetRuntimeMethod(nameof(TimestampedDictionary<int, int>.Add), new[] { typeKey, typeValue });

            reader.Read();
            while (reader.TokenType != JsonToken.EndObject)
            {
                string key = reader.Value.ToString();
                reader.Read();
                if (key == "last")
                {
                    propLast.SetValue(result, serializer.Deserialize<long>(reader));
                }
                else
                {
                    methodAdd.Invoke(result, new[] { key, serializer.Deserialize(reader, typeValue) });
                }
                reader.Read();
            }

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
