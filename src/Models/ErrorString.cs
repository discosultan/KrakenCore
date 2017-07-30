using System;
using System.Text.RegularExpressions;

namespace KrakenCore.Models
{
    public class ErrorString
    {
        public const string Format = @"^(E|W)(\w|\s)+:(\w|\s)+(:(\w|\s)+)?$";

        public const char SeverityCodeError = 'E';
        public const char SeverityCodeWarning = 'W';

        public const string ErrorCategoryGeneral = "General";
        public const string ErrorCategoryOrder = "Order";
        public const string ErrorCategoryQuery = "Query";
        public const string ErrorCategoryService = "Service";
        public const string ErrorCategoryTrade = "Trade";

        public ErrorString(string error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            if (!IsValid(error))
                throw new ArgumentException($"{nameof(error)} has an invalid format. Valid format: {Format}");

            Value = error;
            SeverityCode = error[0];

            string[] errorParts = error.Substring(1).Split(':');
            ErrorCategory = errorParts[0];
            ErrorType = errorParts[1];
            ExtraInfo = errorParts.Length == 3 ? errorParts[2] : null;
        }

        public string Value { get; }
        public char SeverityCode { get; }
        public string ErrorCategory { get; }
        public string ErrorType { get; }
        public string ExtraInfo { get; } // Nullable

        public override string ToString() => Value;

        public static bool IsValid(string error) => Regex.IsMatch(error, Format);

        public static explicit operator ErrorString(string error) => new ErrorString(error);
    }
}