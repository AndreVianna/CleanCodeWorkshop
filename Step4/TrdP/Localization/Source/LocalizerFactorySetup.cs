using System;
using System.Collections.Generic;
using System.Globalization;

namespace TrdP.Localization
{
    public abstract class LocalizerFactorySetup<TSharedResourceProvider>
    {
        public ICollection<CultureInfo> AvailableCultures { get; } = new HashSet<CultureInfo>();
        public Type SharedResourcesLocator => typeof(TSharedResourceProvider);
    }
}