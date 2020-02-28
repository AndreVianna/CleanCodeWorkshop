namespace TrdP.Localization.Abstractions
{
    public interface IStringLocalizer<T> : IStringLocalizer
    {
    }

    public interface IStringLocalizer
    {
        LocalizedString this[string name] { get; }

        LocalizedString this[string name, params object[] arguments] { get; }
    }
}