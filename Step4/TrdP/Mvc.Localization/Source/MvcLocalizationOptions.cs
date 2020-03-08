using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TrdP.Mvc.Localization
{
    public class MvcLocalizationOptions
    {
        public MvcLocalizationOptions()
        {
            AvailableCultures = new List<CultureInfo>();
        }

        public List<CultureInfo> AvailableCultures { get; }
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