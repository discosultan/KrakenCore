using System;
using System.Threading.Tasks;
using KrakenCore.Utils;
using Xunit;

namespace KrakenCore.Tests.Utils
{
    public class RateLimiterTests
    {
        const int CallLimit = 2;
        static readonly TimeSpan CallCounterDecreaseTime = TimeSpan.FromTicks(1);

        readonly ManualStopwatch _stopwatch;
        readonly RateLimiter _rateLimiter;

        public RateLimiterTests()
        {
            _stopwatch = new ManualStopwatch();
            _rateLimiter = new RateLimiter(
                limit: CallLimit,
                decreaseTime: CallCounterDecreaseTime,
                stopwatch: _stopwatch);
        }

        [Fact]
        public async Task WaitAccess_NotLimited()
        {
            Assert.False(await _rateLimiter.WaitAccess(CallLimit));
        }

        [Fact]
        public async Task WaitAccess_Limited()
        {
            await _rateLimiter.WaitAccess(1);
            Assert.True(await _rateLimiter.WaitAccess(CallLimit));
        }

        [Fact]
        public async Task WaitAccess_EnoughTimePassed_NotLimited()
        {
            await _rateLimiter.WaitAccess(CallLimit);
            _stopwatch.Elapsed = CallCounterDecreaseTime;
            Assert.False(await _rateLimiter.WaitAccess(1));
        }

        [Fact]
        public async Task WaitAccess_DoubleRateHalfTimePassed_Limited()
        {
            await _rateLimiter.WaitAccess(CallLimit);
            _stopwatch.Elapsed = CallCounterDecreaseTime;
            Assert.True(await _rateLimiter.WaitAccess(2));
        }
    }

    class ManualStopwatch : IStopwatch
    {
        public TimeSpan Elapsed { get; set; }

        public void Restart() => Elapsed = TimeSpan.Zero;
    }
}
