using System.ComponentModel.DataAnnotations;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators;

namespace TrdP.Mvc.DataAnnotations.Localization.Providers
{
    public interface IValidationAttributeAdapterProvider
    {
        IValidationAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer);
    }
}