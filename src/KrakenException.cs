using System;
using System.Collections.ObjectModel;

namespace KrakenCore
{
    /// <summary>
    /// Represents errors that occur at Kraken API level.
    /// </summary>
    public class KrakenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KrakenException"/> class.
        /// </summary>
        /// <param name="errors">Errors returned by Kraken API.</param>
        /// <param name="message">Message of the exception.</param>
        public KrakenException(ReadOnlyCollection<ErrorString> errors, string message)
            : base(message)
        {
            Errors = errors;
        }

        /// <summary>
        /// Gets the errors returned by Kraken API.
        /// </summary>
        public ReadOnlyCollection<ErrorString> Errors { get; }
    }
}
