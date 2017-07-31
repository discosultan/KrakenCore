using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class OhlcData
    {
        public Dictionary<string, Ohlc> Ohcls { get; set; }

        /// <summary>
        /// Id to be used as since when polling for new, committed OHLC data.
        /// </summary>
        public long Last { get; set; }
    }

    public class Ohlc
    {
        /// <summary>
        /// Unix timestamp.
        /// </summary>
        public long Time { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        /// <summary>
        /// Volume-weighted average price.
        /// </summary>
        public decimal Vwap { get; set; }

        public decimal Volume { get; set; }

        public int Count { get; set; }
    }
}