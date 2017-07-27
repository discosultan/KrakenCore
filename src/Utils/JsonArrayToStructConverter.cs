using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace KrakenCore.Utils
{
    class JsonArrayToStructConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = Activator.CreateInstance(objectType);
            var array = JArray.Load(reader);
            FieldInfo[] fields = objectType.GetRuntimeFields().ToArray();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                JToken token = array[i];
                field.SetValue(result, token.ToObject(field.FieldType));
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
