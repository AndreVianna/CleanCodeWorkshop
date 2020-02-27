using System;
using System.Globalization;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class StringLocalizer<TResourceSource> : IStringLocalizer<TResourceSource>
    {
        private readonly IStringLocalizer _localizer;

        public StringLocalizer(IStringLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _localizer = factory.Create(typeof(TResourceSource));
        }

        public virtual LocalizedString this[string name] => _localizer[name];

        public virtual LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public IStringLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);
    }
}