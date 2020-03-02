using System;
using NSubstitute;
using TrdP.Localization.Abstractions;
using TrdP.UnitTestResources;
using Xunit;

namespace TrdP.Mvc.Localization.Tests
{
    public class HtmlLocalizerFactoryTests
    {
        private readonly IStringLocalizerFactory _mockedStringLocalizerFactory;

        public HtmlLocalizerFactoryTests()
        {
            var mockedStringLocalizer = Substitute.For<IStringLocalizer>();
            _mockedStringLocalizerFactory = Substitute.For<IStringLocalizerFactory>();
            _mockedStringLocalizerFactory.Create<TestResources>().Returns(mockedStringLocalizer);
        }

        [Fact]
        public void HtmlLocalizerFactory_Constructor_ShouldPass()
        {
            var _ = new HtmlLocalizerFactory(_mockedStringLocalizerFactory);
        }

        [Fact]
        public void HtmlLocalizerFactory_Constructor_WithNullStringLocalizerFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new HtmlLocalizerFactory(null));
        }

        [Fact]
        public void HtmlLocalizerFactory_Create_WithType_ShouldPass()
        {
            var factory = new HtmlLocalizerFactory(_mockedStringLocalizerFactory);
            var result = factory.Create<TestResources>();
            Assert.NotNull(result);
            _mockedStringLocalizerFactory.Received().Create<TestResources>();
        }
    }
}
