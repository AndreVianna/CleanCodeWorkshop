namespace TrdP.Localization.Abstractions
{
    // ReSharper disable once UnusedTypeParameter - Used during implementation
    public interface IStringLocalizer<TResourcesLocator> : ILocalizer<LocalizedString>
        where TResourcesLocator : class
    {
    }

    public interface IStringLocalizer : ILocalizer<LocalizedString>
    {
        void SetResourcesFileRelativePath(string resourcesLocatorRelativePath);
    }
}