using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrakenCore.Utils
{
    static class DictionaryExtensions
    {
        public static string ToQueryString(this Dictionary<string, object> args)
        {
            return args.Where(arg => arg.Value != null).Aggregate(
                new StringBuilder(),
                (builder, arg) => builder.Append(builder.Length == 0 ? '?' : '&')
                                         .Append(arg.Key)
                                         .Append('=')
                                         .Append(arg.Value)
            ).ToString();
        }
    }
}
