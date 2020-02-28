using System;
using NSubstitute;
using TrdP.Localization.Abstractions;
using TrdP.Localization.TestData;
using Xunit;

namespace TrdP.Localization.Mvc.Tests
{
    public class HtmlLocalizerFactoryTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.Localization.TestData";
        private readonly IStringLocalizerFactory _mockedStringLocalizerFactory;

        public HtmlLocalizerFactoryTests()
        {
            var mockedStringLocalizer = Substitute.For<IStringLocalizer>();
            _mockedStringLocalizerFactory = Substitute.For<IStringLocalizerFactory>();
            _mockedStringLocalizerFactory.Create(Arg.Any<Type>()).Returns(mockedStringLocalizer);
            _mockedStringLocalizerFactory.Create(Arg.Any<string>(), Arg.Any<string>()).Returns(mockedStringLocalizer);
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
            var result = factory.Create(typeof(TestResources));
            Assert.NotNull(result);
            _mockedStringLocalizerFactory.Received().Create(typeof(TestResources));
        }

        [Fact]
        public void HtmlLocalizerFactory_Create_WithSourceAndAssembly_ShouldPass()
        {
            var factory = new HtmlLocalizerFactory(_mockedStringLocalizerFactory);
            var result = factory.Create("TestResources", SOURCE_ASSEMBLY_NAME);
            Assert.NotNull(result);
            _mockedStringLocalizerFactory.Received().Create("TestResources", SOURCE_ASSEMBLY_NAME);
        }
    }
}
