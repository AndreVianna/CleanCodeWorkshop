using System;
using System.IO;
using System.Text.Encodings.Web;
using Xunit;

namespace TrdP.Localization.Mvc.Abstractions.Tests
{
    public class LocalizedHtmlContentTests
    {
        [Fact]
        public void LocalizedHtmlContent_Constructor_WithNullName_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalizedHtmlContent(null, null));
        }

        [Fact]
        public void LocalizedHtmlContent_Constructor_WithNullValue_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalizedHtmlContent("SomeName", null));
        }

        [Fact]
        public void LocalizedHtmlContent_Constructor_WithNullArguments_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalizedHtmlContent("SomeName", "SomeValue", null));
        }

        [Fact]
        public void LocalizedHtmlContent_WriteTo_WithNullWriter_ShouldThrow()
        {
            var subject = new LocalizedHtmlContent("SomeName", "SomeValue");
            Assert.Throws<ArgumentNullException>(() => subject.WriteTo(null, null));
        }

        [Fact]
        public void LocalizedHtmlContent_WriteTo_WithNullHtmlEncoder_ShouldThrow()
        {
            var subject = new LocalizedHtmlContent("SomeName", "SomeValue");
            using var writer = new StringWriter();
            Assert.Throws<ArgumentNullException>(() => subject.WriteTo(writer, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("SomeValue")]
        [InlineData("<b>SomeValue</b>")]
        public void LocalizedHtmlContent_WriteTo_ShouldPass(string expectedResult)
        {
            var subject = new LocalizedHtmlContent("SomeName", expectedResult);
            using var writer = new StringWriter();
            subject.WriteTo(writer, HtmlEncoder.Default);
            Assert.Equal(expectedResult, writer.ToString());
        }

        [Fact]
        public void LocalizedHtmlContent_WriteTo_WithArguments_ShouldPass()
        {
            var subject = new LocalizedHtmlContent("SomeName", "<b>SomeVale {0}</b>", new object[] { 3 });
            using var writer = new StringWriter();
            subject.WriteTo(writer, HtmlEncoder.Default);
            Assert.Equal("<b>SomeVale 3</b>", writer.ToString());
        }
    }
}
