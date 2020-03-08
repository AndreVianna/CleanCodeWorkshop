using System;
using Xunit;

namespace TrdP.Localization.Abstractions.Tests
{
    public class NullStringLocalizerFactoryTests
    {
        [Fact]
        public void NullStringLocalizerFactory_Instance_ShouldPass()
        {
            var _ = NullStringLocalizerFactory.Instance;
        }

        [Fact]
        public void NullStringLocalizerFactory_AllCreateMethods_ShouldPass()
        {
            Assert.Equal(NullStringLocalizer.Instance, NullStringLocalizerFactory.Instance.Create<Type>());
            Assert.Equal(NullStringLocalizer.Instance, NullStringLocalizerFactory.Instance.Create(typeof(object)));
            Assert.Equal(NullStringLocalizer.Instance, NullStringLocalizerFactory.Instance.Create("SomePath"));
            Assert.Equal(NullStringLocalizer.Instance, NullStringLocalizerFactory.Instance.CreateForSharedResources());
        }
    }
}