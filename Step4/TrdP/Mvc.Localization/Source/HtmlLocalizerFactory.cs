using System;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.Localization.Abstractions;

namespace TrdP.Mvc.Localization
{
    public class HtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private readonly IStringLocalizerFactory _factory;

        public HtmlLocalizerFactory(IStringLocalizerFactory localizerFactory)
        {
            _factory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        }

        public IHtmlLocalizer Create<TResourcesLocator>()
            where TResourcesLocator : class
        {
            return new HtmlLocalizer(_factory.Create<TResourcesLocator>());
        }
    }
}