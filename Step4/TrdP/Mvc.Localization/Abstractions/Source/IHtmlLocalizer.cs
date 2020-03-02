using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.Localization.Abstractions
{
    // ReSharper disable once UnusedTypeParameter - Used during implementation
    public interface IHtmlLocalizer<TResourcesLocator> : ILocalizer<LocalizedHtmlContent>
        where TResourcesLocator : class
    {
    }

    public interface IHtmlLocalizer : ILocalizer<LocalizedHtmlContent>
    {
    }
}