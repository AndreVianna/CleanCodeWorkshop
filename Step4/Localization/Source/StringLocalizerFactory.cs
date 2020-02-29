using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _resourcesRootName;
        private readonly ConcurrentDictionary<string, IStringLocalizer> _localizerCache = new ConcurrentDictionary<string, IStringLocalizer>();

        public StringLocalizerFactory(IOptions<LocalizerOptions> localizationOptions, ILoggerFactory loggerFactory)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _resourcesRootName = localizationOptions.Value.ResourcesRoot ?? string.Empty;
            if (!string.IsNullOrEmpty(_resourcesRootName))
            {
                _resourcesRootName = $"{ConvertToResourceName(_resourcesRootName)}.";
            }
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            return GetOrCreateLocalizer(resourceSource.Assembly, resourceSource.FullName);
        }

        public IStringLocalizer Create(string sourceName, string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceName));
            }

            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(assemblyName));
            }

            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            return GetOrCreateLocalizer(assembly, ConvertToResourceName(sourceName));
        }

        private IStringLocalizer GetOrCreateLocalizer(Assembly assembly, string sourceName)
        {
            var assemblyName = new AssemblyName(assembly.FullName).Name;
            var sourceRelativeName = TrimPrefix(sourceName, $"{assemblyName}.");
            var resourceRelativeName = $"{_resourcesRootName}{sourceRelativeName}";
            return _localizerCache.GetOrAdd(resourceRelativeName, key => new StringLocalizer(assembly, key, _loggerFactory.CreateLogger<StringLocalizer>()));
        }

        private static string ConvertToResourceName(string resourcePath)
        {
            return resourcePath
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
        }

        private static string TrimPrefix(string name, string prefix)
        {
            return name.StartsWith(prefix, StringComparison.Ordinal)
                ? name.Substring(prefix.Length)
                : name;
        }
    }
}