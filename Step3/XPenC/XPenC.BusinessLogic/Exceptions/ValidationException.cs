using System;
using System.Collections.Generic;
using XPenC.BusinessLogic.Validation;

namespace XPenC.BusinessLogic.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
        {
            Operation = "";
            Errors = new List<ValidationError>();
        }

        private ValidationException(string message) : base(message)
        {
        }

        private ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ValidationException(string operation, ValidationError[] errors) : this(GenerateExceptionMessage(operation, errors))
        {
            Operation = operation;
            Errors = new List<ValidationError>(errors);
        }

        public ValidationException(string operation, ValidationError[] errors, Exception innerException) : this(GenerateExceptionMessage(operation, errors), innerException)
        {
            Operation = operation;
            Errors = new List<ValidationError>(errors);
        }

        public string Operation { get; }
        public ICollection<ValidationError> Errors { get; }

        private static string GenerateExceptionMessage(string operation, IEnumerable<ValidationError> errors)
        {
            return $"{operation}: {string.Join("\n", errors)}";
        }
    }
}