namespace TrdP.Localization.Mvc.Abstractions
{
    public interface IHtmlLocalizer<TResource> : IHtmlLocalizer
    {
    }

    public interface IHtmlLocalizer
    {
        LocalizedHtmlContent this[string name] { get; }

        LocalizedHtmlContent this[string name, params object[] arguments] { get; }
    }
}