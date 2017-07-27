using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace KrakenCore.Utils
{
    class RateLimiter
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly int _counterLimit;
        private readonly TimeSpan _counterDecreaseTime;

        private int _callCounter;

        public RateLimiter(int limit, TimeSpan decreaseTime)
        {
            _counterLimit = limit;
            _counterDecreaseTime = decreaseTime;
        }

        public async Task WaitAccess()
        {

        }
    }
}
