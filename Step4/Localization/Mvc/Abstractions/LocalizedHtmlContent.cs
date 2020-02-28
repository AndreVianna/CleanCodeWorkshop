using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace TrdP.Localization.Mvc.Abstractions
{
    public class LocalizedHtmlContent : IHtmlContent
    {
        private readonly object[] _arguments;

        public LocalizedHtmlContent(string name, string value, bool isResourceNotFound = false, string searchedLocation = null)
            : this(name, value, Array.Empty<object>(), isResourceNotFound, searchedLocation)
        {
        }

        public LocalizedHtmlContent(string name, string value, object[] arguments, bool isResourceNotFound = false, string searchedLocation = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            IsResourceNotFound = isResourceNotFound;
            SearchedLocation = searchedLocation;
        }

        public string Name { get; }

        public string Value { get; }

        public bool IsResourceNotFound { get; }

        public string SearchedLocation { get; }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            var formattableString = new HtmlFormattableString(Value, _arguments);
            formattableString.WriteTo(writer, encoder);
        }
    }
}