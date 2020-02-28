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
    public class StringLocalizer<TResourceSource> : IStringLocalizer<TResourceSource> where TResourceSource : class
    {
        private readonly IStringLocalizer _localizer;

        public StringLocalizer(IStringLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _localizer = factory.Create(typeof(TResourceSource));
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];
    }

    public class StringLocalizer : IStringLocalizer
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _sourcePath;
        private readonly ILogger _logger;

        private readonly HashSet<string> _missingNames = new HashSet<string>();

        public StringLocalizer(Assembly assembly, string resourceFileRelativePath, ILogger logger = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrWhiteSpace(resourceFileRelativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourceFileRelativePath));
            }

            _logger = logger ?? NullLogger.Instance;

            var assemblyPath = assembly.GetName().Name;
            _sourcePath = $"{assemblyPath}.{resourceFileRelativePath}";
            _resourceManager = new ResourceManager(_sourcePath, assembly);
        }

        public LocalizedString this[string name] => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments, (k, v, a) => string.Format(v ?? k, a));

        private LocalizedString GetLocalizedString(string name, object[] arguments = null, Func<string, string, object[], string> format = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var value = GetStringOrDefault(name);
            var output = format?.Invoke(name, value, arguments) ?? value ?? name;
            return new LocalizedString(name, output, value == null, $"{_sourcePath}.{CultureInfo.CurrentUICulture.Name}.resx");
        }

#pragma warning disable CA1031 // Do not catch general exception types
        private string GetStringOrDefault(string name)
        {
            var culture = CultureInfo.CurrentUICulture;
            AddLog($"{nameof(StringLocalizer)}: Searching for '{name}' in '{_sourcePath}.{culture.Name}'.");

            var cacheKey = $"culture={culture.Name};name={name}";
            if (_missingNames.Contains(cacheKey))
            {
                AddLog($"{nameof(StringLocalizer)}: '{name}' not found. (from cached missing names)");
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

            void AddLog(string message) => _logger.LogDebug(GetStringOrDefaultEventId(), message);
            static EventId GetStringOrDefaultEventId() => new EventId(1, nameof(GetStringOrDefault));
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}