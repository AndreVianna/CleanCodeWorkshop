using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly HashSet<string> _missingNames = new HashSet<string>();

        private Assembly _resourcesAssembly;
        private string _resourcesAssemblyName;
        private string _resourcesSourceRelativePath;
        private ResourceManager _resourceManager;

        private readonly ILogger _logger;

        public StringLocalizer(Assembly assembly, string resourcesSourceRelativePath, ILogger logger = null)
        {
            SetResourcesSource(assembly, resourcesSourceRelativePath);
            _logger = logger ?? NullLogger.Instance;
        }

        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetFormattedLocalizedString(name, arguments);

        private string ResourcesSourceAbsolutePath => $"{_resourcesAssemblyName}.{_resourcesSourceRelativePath}";

        public void SetResourcesSource(string resourcesSourceRelativePath)
        {
            SetResourcesSource(_resourcesAssembly, resourcesSourceRelativePath);
        }

        private void SetResourcesSource(Assembly assembly, string resourcesSourceRelativePath)
        {
            _resourcesAssembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _resourcesAssemblyName = _resourcesAssembly.GetName().Name;

            if (string.IsNullOrWhiteSpace(resourcesSourceRelativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourcesSourceRelativePath));
            }
            _resourcesSourceRelativePath = ConvertToResourceName(resourcesSourceRelativePath);
            _resourceManager = new ResourceManager(ResourcesSourceAbsolutePath, _resourcesAssembly);
            _missingNames.Clear();
        }

        private static string ConvertToResourceName(string resourcePath)
        {
            //Expected to convert: '/path1/path2/filename.resx' to 'path1.path2.filename'
            //Should also accept: '\\path1\\path2\\filename'
            //Should also accept: 'path1.path2.filename'
            return resourcePath
                .Replace(".resx", "")
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.')
                .TrimStart('.');
        }

        private LocalizedString GetFormattedLocalizedString(string name, params object[] arguments)
        {
            var result = GetLocalizedString(name);
            return new LocalizedString(result.Name, string.Format(result.Value, arguments), result.ResourceWasNotFound, result.SearchedLocation);
        }

        private LocalizedString GetLocalizedString(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var culture = CultureInfo.CurrentUICulture;
            var searchedLocation = $"{ResourcesSourceAbsolutePath}.{culture.Name}.resx";
            AddLog($"{nameof(StringLocalizer)}: Searching for '{name}' in '{searchedLocation}'.");

            var value = GetStringOrDefault(name, culture);
            return new LocalizedString(name, value ?? name, value == null, searchedLocation);
        }

#pragma warning disable CA1031 // Do not catch general exception types
        private string GetStringOrDefault(string name, CultureInfo culture)
        {
            var cacheKey = $"culture={culture.Name};name={name}";
            if (_missingNames.Contains(cacheKey))
            {
                AddLog($"{nameof(StringLocalizer)}: '{name}' retrieved from missing names cache.");
                return null;
            }

            try
            {
                var result = _resourceManager.GetString(name, culture);
                AddLog($"{nameof(StringLocalizer)}: '{name}' not found.");
                return result;
            }
            catch (MissingManifestResourceException)
            {
                AddLog($"{nameof(StringLocalizer)}: '{name}' found.");
                _missingNames.Add(cacheKey);
                return null;
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types

        private void AddLog(string message) => _logger.LogDebug(GetStringOrDefaultEventId(), message);
        private static EventId GetStringOrDefaultEventId() => new EventId(1, nameof(GetStringOrDefault));
    }
}