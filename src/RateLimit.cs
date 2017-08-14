using System;

namespace KrakenCore
{
    /// <summary>
    /// Specifies which type of rate limit should be applied to the client.
    /// <para>For more info, please refer to Kraken API help page.</para>
    /// </summary>
    [Flags]
    public enum RateLimit
    {
        /// <summary>
        /// No limiting.
        /// </summary>
        None  = 0,
        /// <summary>
        /// Limiting based on tier 2 account rules.
        /// </summary>
        Tier2 = 2,
        /// <summary>
        /// Limiting based on tier 3 account rules.
        /// </summary>
        Tier3 = 3,
        /// <summary>
        /// Limiting based on tier 4 account rules.
        /// </summary>
        Tier4 = 4
    }
}
