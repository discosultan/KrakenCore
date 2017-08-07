using KrakenCore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace KrakenCore.Utils
{
    internal class OhlcDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new OhlcData();

            reader.Read(); // StartObject.
            reader.Read(); // PropertyName of OHLC.
            result.Values = serializer.Deserialize<Ohlc[]>(reader);
            reader.Read(); // EndArray.
            reader.Read(); // PropertyName of Last.
            result.Last = serializer.Deserialize<long>(reader);
            reader.Read(); // EndObject.

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }

    internal class JObjectToWrappedDictionaryConverter : JsonConverter
    {
        //private readonly string _dictName;

        public JObjectToWrappedDictionaryConverter()
        {
            //_dictName = dictName;
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = Activator.CreateInstance(objectType);
            var jObj = JObject.Load(reader);

            var props = objectType.GetRuntimeProperties().ToArray();
            //var dictProp = props.First(x => x.Name == _dictName);

            //Type dictValueType = dictProp.PropertyType.GenericTypeArguments[1];

            //var jContract = serializer.ContractResolver.ResolveContract(objectType);
            //jContract.Converter.

            //foreach (var jProp in jObj)
            //{
            //    jContract.Converter.
            //    props.FirstOrDefault(prop => prop.Name == jProp.)

                
            //    if (x.Ke)
            //}

            //FieldInfo[] fields = objectType.GetRuntimeFields().ToArray();
            //for (int i = 0; i < fields.Length; i++)
            //{
            //    FieldInfo field = fields[i];
            //    JToken token = array[i];
            //    field.SetValue(result, token.ToObject(field.FieldType));
            //}
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}