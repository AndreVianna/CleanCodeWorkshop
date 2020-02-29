using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TrdP.Localization.Mvc.Abstractions;

namespace TrdP.Localization.Mvc
{
    public class ViewLocalizer : IViewLocalizer, IViewContextAware
    {
        private readonly IHtmlLocalizer _localizer;

        public ViewLocalizer(IHtmlLocalizer localizer)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public LocalizedHtmlContent this[string name] => _localizer[name];

        public LocalizedHtmlContent this[string name, params object[] arguments] => _localizer[name, arguments];

        public void Contextualize(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            var viewPath = viewContext.ExecutingFilePath;
            if (string.IsNullOrEmpty(viewPath))
            {
                viewPath = viewContext.View.Path;
            }

            Debug.Assert(!string.IsNullOrEmpty(viewPath), "Couldn't determine a path for the view");
            SetResourcesSource(viewPath);
        }
        private void SetResourcesSource(string newResourcesSourceRelativePath) => _localizer.SetResourcesSource(newResourcesSourceRelativePath);
    }
}