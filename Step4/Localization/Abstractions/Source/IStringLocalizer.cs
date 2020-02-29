namespace TrdP.Localization.Abstractions
{
    // ReSharper disable once UnusedTypeParameter - Used during implementation
    public interface IStringLocalizer<TResourcesSource> : ITextLocalizer<LocalizedString>, ITypedResourcesSourceSetter
        where TResourcesSource : class
    {
    }

    public interface IStringLocalizer : ITextLocalizer<LocalizedString>, IResourcesSourceSetter
    {
    }

    public interface ITypedResourcesSourceSetter
    {
        void SetResourcesSource<TNewResourcesSource>() where TNewResourcesSource : class;
    }

    public interface IResourcesSourceSetter
    {
        void SetResourcesSource(string resourcesSourceRelativePath);
    }

    public interface ITextLocalizer<out TLocalizedResult>
    {
        TLocalizedResult this[string name] { get; }

        TLocalizedResult this[string name, params object[] arguments] { get; }
    }
}