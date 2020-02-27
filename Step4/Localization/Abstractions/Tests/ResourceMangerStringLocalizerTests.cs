using System;
using TrdP.Localization.Abstractions;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class LocalizedStringTests
    {
        [Fact]
        public void LocalizedString_Constructor_WithNullName_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalizedString(null, null));
        }

        [Fact]
        public void LocalizedString_Constructor_WithNullValue_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalizedString("SomeName", null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("SomeValue")]
        public void LocalizedString_ImplicitOperatorString_ShouldPass(string expectedResult)
        {
            var subject = new LocalizedString("SomeName", expectedResult);
            Assert.Equal(expectedResult, subject);
        }

        [Fact]
        public void LocalizedString_ImplicitOperatorString_ForNull_ShouldPass()
        {
            LocalizedString subject = null;
            Assert.Equal((string)null, subject);
        }
    }
}
