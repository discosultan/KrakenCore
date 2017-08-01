using System;

namespace KrakenCore
{
    [Flags]
    public enum RateLimit
    {
        None  = 0,
        Tier2 = 2,
        Tier3 = 3,
        Tier4 = 4
    }
}