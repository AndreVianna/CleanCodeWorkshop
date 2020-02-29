using System;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class StringLocalizer<TResourcesSource> : IStringLocalizer<TResourcesSource> where TResourcesSource : class
    {
        private readonly IStringLocalizerFactory _factory;
        private IStringLocalizer _localizer;

        public StringLocalizer(IStringLocalizerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _localizer = _factory.Create(typeof(TResourcesSource));
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public void SetResourcesSource<TNewResourcesSource>()
            where TNewResourcesSource : class
        {
            _localizer = _factory.Create(typeof(TNewResourcesSource));
        }
    }
}