using Newtonsoft.Json;

namespace KrakenCore.Models
{
    /// <summary>
    /// Server's time. This is to aid in approximating the skew time between the server and client.
    /// </summary>
    public class ServerTime
    {
        [JsonProperty("unixtime")]
        public long UnixTime { get; set; }

        /// <summary>
        /// RFC 1123 time format.
        /// </summary>
        public string Rfc1123 { get; set; }

        public override string ToString() => Rfc1123;
    }
}
