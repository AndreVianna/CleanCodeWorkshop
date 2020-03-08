using System;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal class DummyStringLocalizer : IStringLocalizer
    {
        public virtual LocalizedString this[string name] => throw new NotImplementedException();

        public virtual LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();
    }

    internal class DummyStringLocalizer<T> : IStringLocalizer<T> where T : class
    {
        public virtual LocalizedString this[string name] => throw new NotImplementedException();

        public virtual LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();
    }
}