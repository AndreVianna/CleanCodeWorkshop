using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrdP.Mvc.DataAnnotations.Localization.Attributes
{
    public abstract class ValidationProviderAttribute : Attribute
    {
        public abstract IEnumerable<ValidationAttribute> GetValidationAttributes();
    }
}