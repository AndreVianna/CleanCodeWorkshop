using System;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.Localization.Abstractions;

namespace TrdP.Mvc.Localization
{
    public class HtmlLocalizer<TResourcesLocator> : IHtmlLocalizer<TResourcesLocator> where TResourcesLocator : class
    {
        private readonly IHtmlLocalizer _localizer;

        public HtmlLocalizer(IHtmlLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _localizer = factory.Create<TResourcesLocator>();
        }

        public LocalizedHtmlContent this[string name] => _localizer[name];

        public LocalizedHtmlContent this[string name, params object[] arguments] => _localizer[name, arguments];
    }

    internal class HtmlLocalizer : IHtmlLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public HtmlLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public LocalizedHtmlContent this[string name] => ToHtmlContent(_localizer[name], Array.Empty<object>());

        public LocalizedHtmlContent this[string name, params object[] arguments] => ToHtmlContent(_localizer[name], arguments);

        //public void SetResourcesFileRelativePath(string targetRelativePath) => _localizer.SetResourcesFileRelativePath(targetRelativePath);

        private LocalizedHtmlContent ToHtmlContent(LocalizedString result, object[] arguments) =>
            new LocalizedHtmlContent(result.Name, result.Value, arguments, result.ResourceWasNotFound, result.SearchedLocation);
    }
}