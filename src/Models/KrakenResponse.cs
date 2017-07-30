using Newtonsoft.Json;
using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class KrakenResponse<T>
    {
        [JsonProperty("error")]
        public List<ErrorString> Errors { get; set; }

        public T Result { get; set; } // Nullable
    }
}