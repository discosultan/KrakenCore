using Newtonsoft.Json.Serialization;
using System.Text;

namespace KrakenCore.Utils
{
    class SnakeCasePropertyNamesContractResolved : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var result = new StringBuilder();
            foreach (char c in propertyName)
            {
                if (char.IsUpper(c) && result.Length > 0)
                {
                    result.Append('_');
                    result.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }
    }
}