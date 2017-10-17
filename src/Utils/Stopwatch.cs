using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KrakenCore.Tests")]

namespace KrakenCore.Utils
{
    internal interface IStopwatch
    {
        TimeSpan Elapsed { get; }

        void Restart();
    }

    internal class Stopwatch : IStopwatch
    {
        private readonly System.Diagnostics.Stopwatch _stopwatch
            = new System.Diagnostics.Stopwatch();

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public void Restart() => _stopwatch.Restart();
    }
}
