using System;
using XPenC.DataAccess.Contracts.Exceptions;
using Xunit;

namespace XPenC.DataAccess.Contracts.Tests.Exceptions
{
    public class DataProviderExceptionTests
    {
        [Fact]
        public void DataProviderException_DefaultConstructor_ShouldPass()
        {
            var _ = new DataProviderException();
        }

        [Fact]
        public void DataProviderException_SimpleConstructor_ShouldPass()
        {
            var _ = new DataProviderException("Some Message");
        }

        [Fact]
        public void DataProviderException_FullConstructor_ShouldPass()
        {
            var _ = new DataProviderException("Some Message", new Exception());
        }
    }
}