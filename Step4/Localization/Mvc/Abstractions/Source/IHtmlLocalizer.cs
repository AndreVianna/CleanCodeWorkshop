using TrdP.Localization.Abstractions;

namespace TrdP.Localization.Mvc.Abstractions
{
    // ReSharper disable once UnusedTypeParameter - Used during implementation
    public interface IHtmlLocalizer<TResourcesSource> : ITextLocalizer<LocalizedHtmlContent>, ITypedResourcesSourceSetter
        where TResourcesSource : class
    {
    }

    public interface IHtmlLocalizer : ITextLocalizer<LocalizedHtmlContent>, IResourcesSourceSetter
    {
    }
}