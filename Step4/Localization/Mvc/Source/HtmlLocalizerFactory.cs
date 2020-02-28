using System;
using TrdP.Localization.Abstractions;
using TrdP.Localization.Mvc.Abstractions;

namespace TrdP.Localization.Mvc
{
    public class HtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private readonly IStringLocalizerFactory _factory;

        public HtmlLocalizerFactory(IStringLocalizerFactory localizerFactory)
        {
            _factory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        }

        public virtual IHtmlLocalizer Create(Type resourceSource)
        {
            return new HtmlLocalizer(_factory.Create(resourceSource));
        }

        public virtual IHtmlLocalizer Create(string baseName, string location)
        {
            return new HtmlLocalizer(_factory.Create(baseName, location));
        }
    }
}