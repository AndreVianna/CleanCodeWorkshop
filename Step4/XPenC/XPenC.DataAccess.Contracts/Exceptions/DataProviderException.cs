using System;

namespace XPenC.DataAccess.Contracts.Exceptions
{
    public class DataProviderException : Exception
    {
        public DataProviderException()
        {
        }

        public DataProviderException(string message) : base(message)
        {
        }

        public DataProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}