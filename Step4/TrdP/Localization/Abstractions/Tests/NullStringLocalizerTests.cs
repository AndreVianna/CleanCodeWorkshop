using Xunit;

namespace TrdP.Localization.Abstractions.Tests
{
    public class NullStringLocalizerTests
    {
        [Fact]
        public void NullStringLocalizer_Instance_ShouldPass()
        {
            var _ = NullStringLocalizer.Instance;
        }

        [Fact]
        public void NullStringLocalizer_This_ShouldPass()
        {
            var result = NullStringLocalizer.Instance["name"];
            Assert.Equal("name", result);
        }

        [Fact]
        public void NullStringLocalizer_This_WithArguments_ShouldPass()
        {
            var result = NullStringLocalizer.Instance["name {0}", 3];
            Assert.Equal("name 3", result);
        }
    }
}