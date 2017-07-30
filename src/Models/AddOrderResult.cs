using Newtonsoft.Json;

namespace KrakenCore.Models
{
    public class AddOrderResult
    {
        /// <summary>
        /// Order description info.
        /// </summary>
        [JsonProperty("descr")]
        public AddOrderDescription Description { get; set; }

        /// <summary>
        /// Array of transaction ids for order (if order was added successfully).
        /// </summary>
        [JsonProperty("txid")]
        public string[] TransactionIds { get; set; }
    }

    public class AddOrderDescription
    {
        /// <summary>
        /// Order description.
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// Conditional close order description (if conditional close set).
        /// </summary>
        public string Close { get; set; }
    }
}
