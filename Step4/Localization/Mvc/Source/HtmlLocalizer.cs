using System;
using TrdP.Localization.Abstractions;
using TrdP.Localization.Mvc.Abstractions;

namespace TrdP.Localization.Mvc
{
    public class HtmlLocalizer<TResource> : IHtmlLocalizer<TResource> where TResource : class
    {
        private readonly IHtmlLocalizer _localizer;

        public HtmlLocalizer(IHtmlLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _localizer = factory.Create(typeof(TResource));
        }

        public LocalizedHtmlContent this[string name] => _localizer[name];

        public LocalizedHtmlContent this[string name, params object[] arguments] => _localizer[name, arguments];
    }

    public class HtmlLocalizer : IHtmlLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public HtmlLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public LocalizedHtmlContent this[string name] => ToHtmlContent(_localizer[name], Array.Empty<object>());

        public LocalizedHtmlContent this[string name, params object[] arguments] => ToHtmlContent(_localizer[name], arguments);

        private LocalizedHtmlContent ToHtmlContent(LocalizedString result, object[] arguments) =>
            new LocalizedHtmlContent(result.Name, result.Value, arguments, result.ResourceWasNotFound, result.SearchedLocation);
    }
}