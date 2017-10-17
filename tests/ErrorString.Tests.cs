using Xunit;

namespace KrakenCore.Tests
{
    public class ErrorStringTests
    {
        [Theory]
        [InlineData("EQuery:Unknown asset class", true)]
        [InlineData("Eabc:abc:abc", true)]
        [InlineData("Wabc:abc", true)]
        [InlineData("abc", false)]
        [InlineData("eabc:abc", false)]
        [InlineData("Eabc:abc:", false)]
        [InlineData("Eabc:", false)]
        public void IsValid(string input, bool expectedResult)
        {
            bool isValid = ErrorString.IsValid(input);

            Assert.Equal(expectedResult, isValid);
        }
    }
}
