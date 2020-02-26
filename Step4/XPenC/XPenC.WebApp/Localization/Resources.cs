using System.Collections.Generic;
using System.Globalization;

namespace XPenC.WebApp.Localization
{
    // ReSharper disable once ClassNeverInstantiated.Global - class used to locate assembly.
    public static class Resources
    {
        public static IList<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pt-Br"),
        };
    }
}
