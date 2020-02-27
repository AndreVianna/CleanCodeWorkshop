using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class ResourceManagerStringLocalizer : IStringLocalizer
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _sourcePath;
        private readonly CultureInfo _culture;

        private ResourceManagerStringLocalizer(ResourceManager resourceManager, string sourcePath, CultureInfo culture)
        {
            _culture = culture;
            _resourceManager = resourceManager;
            _sourcePath = sourcePath;
        }

        public ResourceManagerStringLocalizer(Assembly assembly, string resourceFileRelativePath, CultureInfo culture = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrWhiteSpace(resourceFileRelativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourceFileRelativePath));
            }
            _culture = culture;

            var assemblyPath = assembly.GetName().Name;
            _sourcePath = $"{assemblyPath}.{resourceFileRelativePath}";
            _resourceManager = new ResourceManager(_sourcePath, assembly);
        }

        private CultureInfo Culture => _culture ?? CultureInfo.CurrentUICulture;

        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments, (k, v, a) => string.Format(v ?? k, a));
        public IStringLocalizer WithCulture(CultureInfo culture) => new ResourceManagerStringLocalizer(_resourceManager, _sourcePath, culture);

        private LocalizedString GetLocalizedString(string name, object[] arguments = null, Func<string, string, object[], string> format = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var value = GetStringOrDefault(name, Culture);
            var output = format?.Invoke(name, value, arguments) ?? value ?? name;
            return new LocalizedString(name, output, value == null, $"{_sourcePath}.{Culture.Name}.resx");
        }

#pragma warning disable CA1031 // Do not catch general exception types
        private string GetStringOrDefault(string name, CultureInfo culture)
        {
            //_logger.SearchedLocation(name, _resourceBaseName, keyCulture);

            //var cacheKey = $"culture={Culture.Name};name={name}";
            //if (_missingNames.ContainsKey(cacheKey))
            //{
            //    return null;
            //}

            try
            {
                return _resourceManager.GetString(name, culture);
            }
            catch (MissingManifestResourceException)
            {
                //_missingNames.TryAdd(cacheKey, null);
                return null;
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}