using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace KrakenCore
{
    public class KrakenResponse<T>
    {
        [JsonProperty("error")]
        public ReadOnlyCollection<ErrorString> Errors { get; set; }

        public T Result { get; set; } // Nullable
    }
}