using System;

namespace TrdP.Localization.Abstractions
{
    public class LocalizedString
    {
        public LocalizedString(string name, string value, bool resourceWasNotFound = false, string searchedLocation = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ResourceWasNotFound = resourceWasNotFound;
            SearchedLocation = searchedLocation;
        }

        public static implicit operator string(LocalizedString localizedString)
        {
            return localizedString?.Value;
        }

        public string Name { get; }

        public string Value { get; }

        public bool ResourceWasNotFound { get; }

        public string SearchedLocation { get; }

        public override string ToString() => Value;
    }
}