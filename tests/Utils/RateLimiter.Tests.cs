using System;
using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests.Utils
{
    public class RateLimiterTests
    {
        private readonly RateLimiter _rateLimiter;
        private long _delay;

        public RateLimiterTests()
        {
            _rateLimiter = new RateLimiter(
                new RateLimit(2, TimeSpan.FromTicks(1)),
                () => Task.FromResult(new DateTime(_delay, DateTimeKind.Utc)),
                time => { _delay += time.Ticks; return Task.CompletedTask; }
            );
        }

        [Fact]
        public async Task WaitAccess_NotLimited()
        {
            await _rateLimiter.WaitAccess(2);
            Assert.Equal(0, _delay);
        }

        [Fact]
        public async Task WaitAccess_Limited()
        {
            await _rateLimiter.WaitAccess(1);
            await _rateLimiter.WaitAccess(2);
            Assert.Equal(1, _delay);
        }

        [Fact]
        public async Task WaitAccess_EnoughTimePassed_NotLimited()
        {
            await _rateLimiter.WaitAccess(2);
            _delay = 1;
            await _rateLimiter.WaitAccess(1);
            Assert.Equal(1, _delay);
        }

        [Fact]
        public async Task WaitAccess_DoubleRateHalfTimePassed_Limited()
        {
            await _rateLimiter.WaitAccess(2);
            _delay = 1;
            await _rateLimiter.WaitAccess(2);
            Assert.Equal(2, _delay);
        }

        [Fact]
        public async Task WaitAccess_Complex()
        {
            await _rateLimiter.WaitAccess(1);
            Assert.Equal(0, _delay);

            await _rateLimiter.WaitAccess(2);
            Assert.Equal(1, _delay);

            await _rateLimiter.WaitAccess(3);
            Assert.Equal(4, _delay);

            _delay = 5;
            await _rateLimiter.WaitAccess(2);
            Assert.Equal(6, _delay);
        }
    }
}
