using System;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal class FakeStringLocalizerFactory : DummyStringLocalizerFactory
    {
        public override IStringLocalizer Create(Type resourcesLocator) => new FakeStringLocalizer();

        public override IStringLocalizer CreateForSharedResources() => new FakeStringLocalizer();
    }

    internal class DummyStringLocalizerFactory : IStringLocalizerFactory
    {
        public virtual IStringLocalizer Create<TResourcesLocator>() where TResourcesLocator : class => throw new NotImplementedException();
        public virtual IStringLocalizer Create(Type resourcesLocator) => throw new NotImplementedException();
        public virtual IStringLocalizer Create(string resourcesRelativePath) => throw new NotImplementedException();
        public virtual IStringLocalizer CreateForSharedResources() => throw new NotImplementedException();
    }
}