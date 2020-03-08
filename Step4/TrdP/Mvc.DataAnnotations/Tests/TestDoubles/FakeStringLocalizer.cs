using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal class FakeStringLocalizer : DummyStringLocalizer
    {
        public override LocalizedString this[string name] => new LocalizedString(name, name, false, string.Empty);

        public override LocalizedString this[string name, params object[] arguments] => name == null ? LocalizedString.NullLocalizedString : new LocalizedString(name, string.Format(name, arguments), false, string.Empty);
    }

    internal class FakeStringLocalizer<T> : DummyStringLocalizer<T> where T : class
    {
        public override LocalizedString this[string name] => new LocalizedString(name, name, false, string.Empty);

        public override LocalizedString this[string name, params object[] arguments] => name == null ? LocalizedString.NullLocalizedString : new LocalizedString(name, string.Format(name, arguments), false, string.Empty);
    }
}