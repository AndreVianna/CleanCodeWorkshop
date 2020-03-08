using System.Collections.Generic;
using System.Globalization;

namespace TrdP.TestWebAppResources
{
    public sealed class Resources
    {
        public static IEnumerable<CultureInfo> AvailableCultures => new []
        {
            new CultureInfo("en-US"),
            new CultureInfo("pt-Br"),
        };
    }
}
