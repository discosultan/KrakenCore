using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace KrakenCore.Models
{
    public class KrakenResponse<T>
    {
        [JsonProperty("error")]
        public ReadOnlyCollection<ErrorString> Errors { get; set; }

        public T Result { get; set; } // Nullable
    }
}