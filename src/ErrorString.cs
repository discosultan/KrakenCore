using System;
using System.Text.RegularExpressions;

namespace KrakenCore
{
    /// <summary>
    /// Formatted error string returned by Kraken.
    /// </summary>
    public class ErrorString
    {
        /// <summary>
        /// Regex for valid format.
        /// </summary>
        public const string Format = @"^(E|W)(\w|\s)+:(\w|\s)+(:(\w|\s)+)?$";

        /// <summary>
        /// Character representing error severity.
        /// </summary>
        public const char SeverityCodeError = 'E';
        /// <summary>
        /// Character representing warning severity.
        /// </summary>
        public const char SeverityCodeWarning = 'W';

        /// <summary>
        /// String representing general category.
        /// </summary>
        public const string ErrorCategoryGeneral = "General";
        /// <summary>
        /// String representing order category.
        /// </summary>
        public const string ErrorCategoryOrder = "Order";
        /// <summary>
        /// String representing query category.
        /// </summary>
        public const string ErrorCategoryQuery = "Query";
        /// <summary>
        /// String representing service category.
        /// </summary>
        public const string ErrorCategoryService = "Service";
        /// <summary>
        /// String representing trade category.
        /// </summary>
        public const string ErrorCategoryTrade = "Trade";

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorString"/> class.
        /// </summary>
        /// <param name="error">Error string returned by Kraken.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="error"/> does not conform to <see cref="Format"/>.
        /// </exception>
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

        /// <summary>
        /// Gets the raw string value of the error.
        /// </summary>
        public string Value { get; }
        /// <summary>
        /// Gets the error severity.
        /// </summary>
        public char SeverityCode { get; }
        /// <summary>
        /// Gets the error category.
        /// </summary>
        public string ErrorCategory { get; }
        /// <summary>
        /// Gets the error type.
        /// </summary>
        public string ErrorType { get; }
        /// <summary>
        /// Gets any extra information associated with the error.
        /// <para>May be <c>null</c>.</para>
        /// </summary>
        public string ExtraInfo { get; } // Nullable

        /// <summary>
        /// Gets the raw string value of the error.
        /// </summary>
        /// <returns>The raw string representation of the error.</returns>
        public override string ToString() => Value;

        /// <summary>
        /// Validates the format of a string.
        /// </summary>
        /// <param name="error">String to validate.</param>
        /// <returns><c>true</c> if conforms; otherwise <c>false</c>.</returns>
        public static bool IsValid(string error) => Regex.IsMatch(error, Format);

        /// <summary>
        /// Explicitly converts a <see cref="string"/> to an <see cref="ErrorString"/>.
        /// </summary>
        /// <param name="error">Instance to convert.</param>
        public static explicit operator ErrorString(string error) => new ErrorString(error);
    }
}
