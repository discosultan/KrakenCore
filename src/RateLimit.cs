using System;

namespace KrakenCore
{
    [Flags]
    public enum RateLimit
    {
        None,
        Tier2,
        Tier3,
        Tier4
    }
}