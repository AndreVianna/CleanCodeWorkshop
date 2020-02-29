using System;
using TrdP.Localization.Abstractions;
using TrdP.Localization.Mvc.Abstractions;

namespace TrdP.Localization.Mvc
{
    public class HtmlLocalizer : IHtmlLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public HtmlLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public LocalizedHtmlContent this[string name] => ToHtmlContent(_localizer[name], Array.Empty<object>());

        public LocalizedHtmlContent this[string name, params object[] arguments] => ToHtmlContent(_localizer[name], arguments);

        public void SetResourcesSource(string newResourcesSourceRelativePath) => _localizer.SetResourcesSource(newResourcesSourceRelativePath);

        private LocalizedHtmlContent ToHtmlContent(LocalizedString result, object[] arguments) =>
            new LocalizedHtmlContent(result.Name, result.Value, arguments, result.ResourceWasNotFound, result.SearchedLocation);
    }
}