namespace TrdP.Localization.Abstractions
{
    public sealed class NullStringLocalizer : IStringLocalizer
    {
        private NullStringLocalizer()
        {
        }

        public static IStringLocalizer Instance { get; } = new NullStringLocalizer();

        public LocalizedString this[string name] => new LocalizedString(name, name, true, string.Empty);

        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, string.Format(name, arguments), true, string.Empty);
    }
}