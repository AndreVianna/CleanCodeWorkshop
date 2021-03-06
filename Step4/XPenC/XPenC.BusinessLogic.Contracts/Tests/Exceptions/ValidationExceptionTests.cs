using System;
using XPenC.BusinessLogic.Contracts.Exceptions;
using Xunit;

namespace XPenC.BusinessLogic.Contracts.Tests.Exceptions
{
    public class ValidationExceptionTests
    {
        [Fact]
        public void ValidationException_DefaultConstructor_ShouldPass()
        {
            var _ = new ValidationException();
        }

        [Fact]
        public void ValidationException_SimpleConstructor_ShouldPass()
        {
            var _ = new ValidationException("Some Operation", Array.Empty<ValidationError>());
        }

        [Fact]
        public void ValidationException_FullConstructor_ShouldPass()
        {
            var _ = new ValidationException("Some Operation", Array.Empty<ValidationError>(), new Exception());
        }
    }
}