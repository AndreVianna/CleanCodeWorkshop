using Microsoft.Extensions.Localization;

namespace XPenC.UI.Mvc.Tests.TestDoubles
{
    public class FakeStringLocalizer<T> : DummyStringLocalizer<T>
    {
        public override LocalizedString this[string name] => new LocalizedString(name, name);
    }
}