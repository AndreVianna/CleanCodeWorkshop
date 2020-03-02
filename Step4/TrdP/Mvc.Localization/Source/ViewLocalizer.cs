using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TrdP.Mvc.Localization.Abstractions;

namespace TrdP.Mvc.Localization
{
    public class ViewLocalizer : IViewLocalizer, IViewContextAware
    {
        private readonly IHtmlLocalizerFactory _factory;
        private IHtmlLocalizer _localizer;

        public ViewLocalizer(IHtmlLocalizerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public LocalizedHtmlContent this[string name] => _localizer[name];

        public LocalizedHtmlContent this[string name, params object[] arguments] => _localizer[name, arguments];

        public void Contextualize(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            var viewPath = viewContext.ExecutingFilePath?.Trim();
            if (string.IsNullOrEmpty(viewPath))
            {
                viewPath = viewContext.View.Path.Trim();
            }

            if (Path.HasExtension(viewPath))
            {
                viewPath = viewPath.Replace(Path.GetExtension(viewPath), "");
            }

            Debug.Assert(!string.IsNullOrEmpty(viewPath), "Couldn't determine a path for the view");
            _localizer = _factory.Create(viewPath);
        }
    }
}