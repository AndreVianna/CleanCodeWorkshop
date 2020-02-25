using Microsoft.Extensions.Localization;

namespace XPenC.WebApp.Tests.TestDoubles
{
    public class FakeStringLocalizer<T> : DummyStringLocalizer<T>
    {
        public override LocalizedString this[string name] => new LocalizedString(name, name);
    }
}