﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class FileExtensionsAttributeAdapter : ValidationAttributeAdapter<FileExtensionsAttribute>
    {
        private readonly string _extensions;
        private readonly string _formattedExtensions;

        public FileExtensionsAttributeAdapter(FileExtensionsAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            // Build the extension list based on how the JQuery Validation's 'extension' method expects it
            // https://jqueryvalidation.org/extension-method/

            // These lines follow the same approach as the FileExtensionsAttribute.
            var normalizedExtensions = Attribute.Extensions.Replace(" ", string.Empty).Replace(".", string.Empty).ToLowerInvariant();
            var parsedExtensions = normalizedExtensions.Split(',').Select(e => "." + e);
            _formattedExtensions = string.Join(", ", parsedExtensions);
            _extensions = string.Join(",", parsedExtensions);
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-fileextensions", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-fileextensions-extensions", _extensions);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var displayName = validationContext.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, _formattedExtensions);
        }
    }
}