using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace XPenC.WebApp.Tests.TestDoubles
{
    public class DummyStringLocalizer<T> : IStringLocalizer<T>
    {
        public virtual LocalizedString this[string name] => throw new NotImplementedException();

        public virtual LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => throw new NotImplementedException();
        public virtual IStringLocalizer WithCulture(CultureInfo culture) => throw new NotImplementedException();
    }
}