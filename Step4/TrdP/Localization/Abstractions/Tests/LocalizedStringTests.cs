using System;
using Xunit;

namespace TrdP.Localization.Abstractions.Tests
{
    public class LocalizedStringTests
    {
        [Fact]
        public void LocalizedString_Constructor_WithNullName_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalizedString(null, null));
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
