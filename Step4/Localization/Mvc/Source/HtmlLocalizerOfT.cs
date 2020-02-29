using System;
using TrdP.Localization.Mvc.Abstractions;

namespace TrdP.Localization.Mvc
{
    public class HtmlLocalizer<TResourcesSource> : IHtmlLocalizer<TResourcesSource> where TResourcesSource : class
    {
        private readonly IHtmlLocalizerFactory _factory;
        private IHtmlLocalizer _localizer;

        public HtmlLocalizer(IHtmlLocalizerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _localizer = _factory.Create(typeof(TResourcesSource));
        }

        public LocalizedHtmlContent this[string name] => _localizer[name];

        public LocalizedHtmlContent this[string name, params object[] arguments] => _localizer[name, arguments];

        public void SetResourcesSource<TNewResourcesSource>()
            where TNewResourcesSource : class
        {
            _localizer = _factory.Create(typeof(TNewResourcesSource));
        }
    }
}