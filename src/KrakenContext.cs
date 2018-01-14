using System.Net.Http;

namespace KrakenCore
{
    /// <summary>
    /// Request context available for interception. This stands as an extensibility point to apply
    /// custom behavior (such as rate limiting) before the actual request is sent to Kraken.
    /// </summary>
    public class KrakenRequestContext
    {
        /// <summary>
        /// Gets or sets the HTTP request object.
        /// </summary>
        public HttpRequestMessage HttpRequest { get; set; }

        /// <summary>
        /// Gets the cost of this particular call. Useful to implement rate limiting, for example.
        /// </summary>
        public int ApiCallCost { get; internal set; }
    }

    /// <summary>
    /// Response context available for interception. This stands as an extensibility point to apply
    /// custom behavior (such as logging) right after the actual response has been received from Kraken.
    /// </summary>
    public class KrakenResponseContext
    {
        /// <summary>
        /// Gets or sets the HTTP response object.
        /// </summary>
        public HttpResponseMessage HttpResponse { get; set; }
    }
}
