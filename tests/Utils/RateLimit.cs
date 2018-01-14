using System;

namespace KrakenCore.Tests.Utils
{
    /// <summary>
    /// Specifies which type of rate limit should be applied to the client.
    /// <para>For more info, please refer to Kraken API help page.</para>
    /// </summary>
    public class RateLimit
    {
        /// <summary>
        /// Gets the treshold upon which cost will accumulate.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets the time it takes for cost to expire.
        /// </summary>
        public TimeSpan DecreaseTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimit"/> class.
        /// </summary>
        /// <param name="limit">The treshold upon which cost will accumulate.</param>
        /// <param name="decreaseTime">The time it takes for cost to expire.</param>
        public RateLimit(int limit, TimeSpan decreaseTime)
        {
            if (limit < 2) throw new ArgumentException($"Argument {nameof(limit)} cannot be less than 2.");

            Limit = limit;
            DecreaseTime = decreaseTime;
        }

        /// <summary>
        /// Limiting based on tier 2 account rules.
        /// </summary>
        public static RateLimit Tier2 { get; } = new RateLimit(15, TimeSpan.FromSeconds(3));

        /// <summary>
        /// Limiting based on tier 3 account rules.
        /// </summary>
        public static RateLimit Tier3 { get; } = new RateLimit(20, TimeSpan.FromSeconds(2));

        /// <summary>
        /// Limiting based on tier 4 account rules.
        /// </summary>
        public static RateLimit Tier4 { get; } = new RateLimit(20, TimeSpan.FromSeconds(1));
    }
}
