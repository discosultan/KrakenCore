using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("KrakenCore.Tests")]

namespace KrakenCore.Utils
{
    // Not thread safe.
    class RateLimiter
    {
        readonly int _counterLimit;
        readonly TimeSpan _counterDecreaseTime;
        readonly IStopwatch _stopwatch;

        int _callCounter;

        public RateLimiter(int limit, TimeSpan decreaseTime, IStopwatch stopwatch)
        {
            _counterLimit = limit;
            _counterDecreaseTime = decreaseTime;
            _stopwatch = stopwatch;
        }

        public async Task<bool> WaitAccess(int counterIncrease)
        {
            bool waited = false;

            // Time passed since last time access was granted.
            TimeSpan elapsed = _stopwatch.Elapsed;

            while (elapsed >= _counterDecreaseTime)
            {
                _callCounter = Math.Max(_callCounter - 1, 0);
                elapsed -= _counterDecreaseTime;
            }

            if (_callCounter >= _counterLimit)
            {
                var toWait = TimeSpan.FromSeconds(
                    (_callCounter - _counterLimit) * _counterDecreaseTime.TotalSeconds
                ) - elapsed;
                await Task.Delay(toWait);
                waited = true;
            }

            _callCounter += counterIncrease;
            _stopwatch.Restart();

            return waited;
        }
    }
}
