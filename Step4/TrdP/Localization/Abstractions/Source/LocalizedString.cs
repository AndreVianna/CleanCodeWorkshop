using System;

namespace TrdP.Localization.Abstractions
{
    public class LocalizedString
    {
        public LocalizedString(string name, string value, bool resourceWasNotFound = false, string searchedLocation = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
            ResourceWasNotFound = resourceWasNotFound;
            SearchedLocation = searchedLocation;
        }

        public static implicit operator string(LocalizedString localizedString)
        {
            return localizedString?.Value;
        }

        public static LocalizedString NullLocalizedString => new LocalizedString(string.Empty, null, true, string.Empty);

        public static LocalizedString EmptyLocalizedString => new LocalizedString(string.Empty, string.Empty, true, string.Empty);

        public string Name { get; }

        public string Value { get; }

        public bool ResourceWasNotFound { get; }

        public string SearchedLocation { get; }

        public override string ToString() => Value;
    }
}