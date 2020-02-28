namespace TrdP.Localization.Abstractions
{
    // ReSharper disable once UnusedTypeParameter - Used during implementation
    public interface IStringLocalizer<TResource> : IStringLocalizer where TResource : class
    {
    }

    public interface IStringLocalizer
    {
        LocalizedString this[string name] { get; }

        LocalizedString this[string name, params object[] arguments] { get; }
    }
}