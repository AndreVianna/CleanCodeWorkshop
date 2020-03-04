using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TrdP.Mvc.Localization
{
    public class MvcLocalizationOptions
    {
        public List<CultureInfo> AvailableCultures { get; } = new List<CultureInfo>();
        public CultureInfo DefaultUiCulture => AvailableCultures.FirstOrDefault() ?? CultureInfo.CurrentUICulture;

        public void AddCulture(CultureInfo culture)
        {
            AvailableCultures.Add(culture);
        }

        public void AddCultures(IEnumerable<CultureInfo> cultures)
        {
            AvailableCultures.AddRange(cultures);
        }
    }
}