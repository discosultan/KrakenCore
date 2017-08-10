using KrakenCore.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests.Utils
{
    public class RateLimiterTests
    {
        private const int CallLimit = 2;
        private static readonly TimeSpan CallCounterDecreaseTime = TimeSpan.FromTicks(1);

        private readonly ManualStopwatch _stopwatch;
        private readonly RateLimiter _rateLimiter;

        public RateLimiterTests()
        {
            _stopwatch = new ManualStopwatch();
            _rateLimiter = new RateLimiter(CallLimit, CallCounterDecreaseTime, _stopwatch);
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

    internal class ManualStopwatch : IStopwatch
    {
        public TimeSpan Elapsed { get; set; }

        public void Restart() => Elapsed = TimeSpan.Zero;
    }
}