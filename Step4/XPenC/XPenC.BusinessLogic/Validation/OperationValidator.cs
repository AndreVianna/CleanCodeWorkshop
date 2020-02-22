using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Exceptions;

namespace XPenC.BusinessLogic.Validation
{
    public class OperationValidator
    {
        private readonly ICollection<ValidationError> _errors = new List<ValidationError>();
        private readonly string _operation;

        public OperationValidator(string operation)
        {
            _operation = operation;
        }


        public void AddError(string source, string message)
        {
            _errors.Add(new ValidationError(source, message));
        }

        public void Validate()
        {
            if (_errors.Any())
            {
                throw new ValidationException(_operation, _errors.ToArray());
            }
        }
    }
}