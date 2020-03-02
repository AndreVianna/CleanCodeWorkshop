using System.Collections.Generic;
using System.Globalization;

namespace TrdP.TestWebAppResources
{
    public class ResourcesProvider
    {
        public static IList<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pt-Br"),
        };
    }
}
