using System.Threading.Tasks;
using Xunit;

namespace KrakenCore.Tests
{
    public partial class KrakenClientTests
    {
        [Fact]
        public async Task GetAccountBalance()
        {
            var res = await _client.GetAccountBalance();

            Assert.Empty(res.Errors);
        }

        [Fact]
        public async Task Niha()
        {

        }
    }
}
