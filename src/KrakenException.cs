using KrakenCore.Models;
using System;
using System.Collections.ObjectModel;

namespace KrakenCore
{
    public class KrakenException : Exception
    {
        public KrakenException(ReadOnlyCollection<ErrorString> errors, string message)
            : base(message)
        {
            Errors = errors;
        }

        public ReadOnlyCollection<ErrorString> Errors { get; }
    }
}
