using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace TrdP.ResourceManagerStringProvider
{
    public class ResourceManagerStringLocalizer : IStringLocalizer
    {
        private readonly Assembly _assembly;
        private readonly string _resourceFileRelativePath;
        private readonly CultureInfo _culture;

        private readonly ResourceManager _resourceManager;
        private readonly string _sourcePath;

        public ResourceManagerStringLocalizer(Assembly assembly, string resourceFileRelativePath, CultureInfo culture = null)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(resourceFileRelativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourceFileRelativePath));
            }
            _resourceFileRelativePath = resourceFileRelativePath;
            _culture = culture;

            var assemblyPath = _assembly.GetName().Name;
            _sourcePath = $"{assemblyPath}.{resourceFileRelativePath}";
            _resourceManager = new ResourceManager(_sourcePath, assembly);

            //_sourcePath += culture == null ? "" : $".{culture}";
        }

        private CultureInfo Culture => _culture ?? CultureInfo.CurrentUICulture;

        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments, (k, v, a) => string.Format(v ?? k, a));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var resourceNames = includeParentCultures
                ? GetAllNamesFromCultureHierarchy(Culture)
                : GetAllNames(Culture);

            foreach (var name in resourceNames)
            {
                yield return this[name];
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
            => new ResourceManagerStringLocalizer(_assembly, _resourceFileRelativePath);

        private LocalizedString GetLocalizedString(string name, object[] arguments = null, Func<string, string, object[], string> format = null)
        {
            var value = GetStringOrDefault(name, Culture);
            var output = format?.Invoke(name, value, arguments) ?? value ?? name;
            return new LocalizedString(name, output, value == null, $"{_sourcePath}.{Culture.Name}.resx");
        }

        private IEnumerable<string> GetAllNamesFromCultureHierarchy(CultureInfo startingCulture)
        {
            var resourceNames = new List<string>();
            resourceNames.AddRange(GetAllNames(startingCulture));
            while (!startingCulture.Equals(startingCulture.Parent))
            {
                startingCulture = startingCulture.Parent;
                resourceNames.AddRange(GetAllNames(startingCulture));
            }
            return resourceNames;
        }

        private IEnumerable<string> GetAllNames(CultureInfo culture)
        {
            //var cacheKey = $"culture={culture.Name};resourceName={_resourceFileRelativePath};assembly={_assembly.FullName}";
            //return _resourceNamesCache.GetOrAdd(cacheKey, _ =>
            //{
            var resourceSet = _resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
            return resourceSet == null
                ? Enumerable.Empty<string>()
                : from DictionaryEntry entry in resourceSet select (string) entry.Key;
            //});
        }

#pragma warning disable CA1031 // Do not catch general exception types
        private string GetStringOrDefault(string name, CultureInfo culture)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

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