namespace TrdP.Localization.Mvc.Abstractions
{
    // ReSharper disable once UnusedTypeParameter - Used during implementation
    public interface IHtmlLocalizer<TResource> : IHtmlLocalizer where TResource : class
    {
    }

    public interface IHtmlLocalizer
    {
        LocalizedHtmlContent this[string name] { get; }

        LocalizedHtmlContent this[string name, params object[] arguments] { get; }
    }
}