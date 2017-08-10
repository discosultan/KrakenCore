using KrakenCore.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KrakenCore
{
    /// <summary>
    /// Represents a collection of keys and values. Additionally contains a timestamp <see
    /// cref="Last"/> which can be used as <c>since</c> argument when polling for new data.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [JsonConverter(typeof(TimestampedDictionaryConverter))]
    public class TimestampedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// Id to be used as <c>since</c> when polling for new data.
        /// </summary>
        public long Last { get; set; }
    }
}
