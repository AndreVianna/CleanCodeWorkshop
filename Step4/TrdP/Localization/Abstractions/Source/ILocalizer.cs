namespace TrdP.Localization.Abstractions
{
    public interface ILocalizer<out TLocalizedResult>
    {
        TLocalizedResult this[string name] { get; }

        TLocalizedResult this[string name, params object[] arguments] { get; }
    }
}