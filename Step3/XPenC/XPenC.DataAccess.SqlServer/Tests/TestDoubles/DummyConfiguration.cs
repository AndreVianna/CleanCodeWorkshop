using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace XPenC.DataAccess.SqlServer.Tests.TestDoubles
{
    internal class DummyConfiguration : IConfiguration
    {
        public virtual string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IConfigurationSection> GetChildren() => throw new NotImplementedException();
        public IChangeToken GetReloadToken() => throw new NotImplementedException();
        public IConfigurationSection GetSection(string key) => throw new NotImplementedException();
    }
}
