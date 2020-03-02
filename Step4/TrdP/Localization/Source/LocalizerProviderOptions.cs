using System.Collections.Generic;
using System.Globalization;

namespace TrdP.Localization
{
    public class LocalizerProviderOptions
    {
        public string ResourcesRoot { get; set; } = string.Empty;
        public ICollection<CultureInfo> AvailableCultures { get; set; } = new HashSet<CultureInfo>();
    }
}