using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class RecentSpreads
    {
        public Dictionary<string, Spread[]> Spreads { get; set; }

        /// <summary>
        /// Id to be used as since when polling for new spread data.
        /// </summary>
        public long Last { get; set; }
    }

    public class Spread
    {
        public int Time { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }
    }
}