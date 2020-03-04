using System.Collections.Generic;
using System.Globalization;

namespace TrdP.TestWebAppResources
{
    public class Resources
    {
        public static IList<CultureInfo> AvailableCultures => new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pt-Br"),
        };
    }
}
