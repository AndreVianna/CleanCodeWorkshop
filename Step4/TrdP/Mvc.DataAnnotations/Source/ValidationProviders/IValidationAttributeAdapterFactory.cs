using System.ComponentModel.DataAnnotations;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    public interface IValidationAttributeAdapterFactory
    {
        IValidationAttributeAdapter Create(ValidationAttribute attribute, IStringLocalizer stringLocalizer);
    }
}