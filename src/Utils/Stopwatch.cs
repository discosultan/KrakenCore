using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("KrakenCore.Tests")]

namespace KrakenCore.Utils
{
    interface IStopwatch
    {
        TimeSpan Elapsed { get; }
        void Restart();
    }

    class Stopwatch : IStopwatch
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public void Restart() => _stopwatch.Restart();
    }
}
