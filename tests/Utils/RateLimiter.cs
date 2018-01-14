using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrakenCore.Tests.Utils
{
    /// <summary>
    /// Enables limiting API calls by specified rate in order not to exhaust Kraken limits.
    /// <para>Not thread-safe!</para>
    /// </summary>
    public class RateLimiter
    {
        // Invert control of timestamp and delay functions to caller.
        private readonly Func<Task<DateTime>> _timestamp;
        private readonly Func<TimeSpan, Task> _delay;

        private readonly LinkedList<long> _queuedResources = new LinkedList<long>();

        /// <summary>
        /// Gets the rate limit used by this limiter.
        /// </summary>
        public RateLimit RateLimit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiter"/> class.
        /// </summary>
        /// <param name="rateLimit">Rate limit settings.</param>
        /// <param name="timestamp">
        /// Function for getting current timestamp. Pass <c>null</c> to use <see cref="DateTime.UtcNow"/>.
        /// </param>
        /// <param name="delay">Function for sleeping. Pass <c>null</c> to use <see cref="Task.Delay"/>.</param>
        public RateLimiter(
            RateLimit rateLimit,
            Func<Task<DateTime>> timestamp = null,
            Func<TimeSpan, Task> delay = null)
        {
            RateLimit = rateLimit ?? throw new ArgumentNullException(nameof(rateLimit));
            _timestamp = timestamp ?? (() => Task.FromResult(DateTime.UtcNow));
            _delay = delay ?? Task.Delay;
        }

        /// <summary>
        /// Checks if the <paramref name="cost"/> goes over rate limit. If it does; waits the
        /// required amount of time for the call to be valid.
        /// </summary>
        /// <param name="cost">Cost of the pending API call.</param>
        /// <returns>A future of the waiting process.</returns>
        public async Task WaitAccess(int cost)
        {
            long timestamp = (await _timestamp()).Ticks;

            // Deque expired costs.
            if (_queuedResources.Count > 0)
            {
                LinkedListNode<long> firstNode = _queuedResources.First;
                while (firstNode.Value <= timestamp)
                {
                    LinkedListNode<long> nextNode = firstNode.Next;
                    _queuedResources.RemoveFirst();
                    if (nextNode == null) break;
                    firstNode = nextNode;
                }
            }

            // Enqueue current cost.
            long last = _queuedResources.Count > 0
                ? _queuedResources.Last.Value
                : timestamp;
            for (int i = 0; i < cost; i++)
            {
                last += RateLimit.DecreaseTime.Ticks;
                _queuedResources.AddLast(last);
            }

            // Wait if we have run over limit.
            int missing = Math.Max(_queuedResources.Count - RateLimit.Limit, 0);
            if (missing > 0)
            {
                LinkedListNode<long> walk = _queuedResources.First;
                for (int i = 1; i < missing; i++)
                    walk = walk.Next;
                await _delay(TimeSpan.FromTicks(walk.Value - timestamp));
            }
        }
    }
}
