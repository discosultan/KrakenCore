using KrakenCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrakenCore
{
    public partial class KrakenClient
    {
        public Task<KrakenResponse<object>> GetAccountBalance()
        {
            return QueryPrivate<object>("/Balance");
        }
    }
}
