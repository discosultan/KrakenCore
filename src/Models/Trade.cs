using System.Collections.Generic;

namespace KrakenCore.Models
{
    public class TradesData
    {
        public Dictionary<string, Trade[]> Trades { get; set; }

        /// <summary>
        /// Id to be used as since when polling for new trade data.
        /// </summary>
        public long Last { get; set; }
    }

    public class Trade
    {
        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public int Time { get; set; }

        public string Side { get; set; }

        public string Type { get; set; }

        public string Misc { get; set; }
    }
}
