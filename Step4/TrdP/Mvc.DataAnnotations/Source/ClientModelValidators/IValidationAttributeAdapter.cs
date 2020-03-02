using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators
{
    public interface IValidationAttributeAdapter : IClientModelValidator
    {
        string GetErrorMessage(ModelValidationContextBase validationContext);
    }
}