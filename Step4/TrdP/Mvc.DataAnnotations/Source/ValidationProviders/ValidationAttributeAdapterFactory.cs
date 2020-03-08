using System;
using System.ComponentModel.DataAnnotations;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    public class ValidationAttributeAdapterFactory : IValidationAttributeAdapterFactory
    {
        public IValidationAttributeAdapter Create(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            return attribute switch
            {
                RegularExpressionAttribute regularExpressionAttribute => (IValidationAttributeAdapter)new RegularExpressionAttributeAdapter(regularExpressionAttribute, stringLocalizer),
                MaxLengthAttribute maxLengthAttribute => (IValidationAttributeAdapter)new MaxLengthAttributeAdapter(maxLengthAttribute, stringLocalizer),
                RequiredAttribute requiredAttribute => (IValidationAttributeAdapter)new RequiredAttributeAdapter(requiredAttribute, stringLocalizer),
                CompareAttribute compareAttribute => (IValidationAttributeAdapter)new CompareAttributeAdapter(compareAttribute, stringLocalizer),
                MinLengthAttribute minLengthAttribute => (IValidationAttributeAdapter)new MinLengthAttributeAdapter(minLengthAttribute, stringLocalizer),
                CreditCardAttribute creditCardAttribute => (IValidationAttributeAdapter)new DataTypeAttributeAdapter(creditCardAttribute, "creditcard", stringLocalizer),
                StringLengthAttribute stringLengthAttribute => (IValidationAttributeAdapter)new StringLengthAttributeAdapter(stringLengthAttribute, stringLocalizer),
                RangeAttribute rangeAttribute => (IValidationAttributeAdapter)new RangeAttributeAdapter(rangeAttribute, stringLocalizer),
                EmailAddressAttribute emailAddressAttribute => (IValidationAttributeAdapter)new DataTypeAttributeAdapter(emailAddressAttribute, "email", stringLocalizer),
                PhoneAttribute phoneAttribute => (IValidationAttributeAdapter)new DataTypeAttributeAdapter(phoneAttribute, "phone", stringLocalizer),
                UrlAttribute urlAttribute => (IValidationAttributeAdapter)new DataTypeAttributeAdapter(urlAttribute, "url", stringLocalizer),
                FileExtensionsAttribute fileExtensionsAttribute => (IValidationAttributeAdapter)new FileExtensionsAttributeAdapter(fileExtensionsAttribute, stringLocalizer),
                _ => null,
            };
        }
    };
}