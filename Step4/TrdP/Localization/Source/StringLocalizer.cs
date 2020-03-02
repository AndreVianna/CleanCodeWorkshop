using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class StringLocalizer<TResourcesLocator> : IStringLocalizer<TResourcesLocator> where TResourcesLocator : class
    {
        private readonly IStringLocalizer _localizer;

        public StringLocalizer(IStringLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _localizer = factory.Create<TResourcesLocator>();
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];
    }

    internal class StringLocalizer : IStringLocalizer
    {
        private readonly HashSet<string> _missingNames = new HashSet<string>();

        private readonly string _resourcesRootPath;
        private readonly string _resourcesAssemblyName;
        private readonly string _resourcesFileRelativePath;
        private readonly ResourceManager _resourceManager;

        private readonly ILogger _logger;

        public StringLocalizer(Assembly assembly, string resourcesRootPath, string resourcesLocatorRelativePath, ILogger logger = null)
        {
            var resourcesAssembly = assembly;
            _resourcesAssemblyName = $"{resourcesAssembly.GetName().Name}.";
            _resourcesRootPath = string.IsNullOrWhiteSpace(resourcesRootPath) ? "" : $"{resourcesRootPath}.";
            _resourcesFileRelativePath = resourcesLocatorRelativePath;
            _resourceManager = new ResourceManager(ResourceFileFinalPath, resourcesAssembly);
            _missingNames.Clear();
            _logger = logger ?? NullLogger.Instance;
        }

        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetFormattedLocalizedString(name, arguments);

        private string ResourceFileFinalPath => $"{_resourcesAssemblyName}{_resourcesRootPath}{_resourcesFileRelativePath}";

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
            var searchedLocation = $"{ResourceFileFinalPath}.{culture.Name}.resx";
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