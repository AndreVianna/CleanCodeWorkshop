using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public class ResourceManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly string _resourcesRootName;
        private readonly ConcurrentDictionary<string, ResourceManagerStringLocalizer> _localizerCache = new ConcurrentDictionary<string, ResourceManagerStringLocalizer>();

        public ResourceManagerStringLocalizerFactory(IOptions<LocalizerOptions> localizationOptions)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            _resourcesRootName = localizationOptions.Value.ResourcesRoot ?? string.Empty;
            if (!string.IsNullOrEmpty(_resourcesRootName))
            {
                _resourcesRootName = $"{ConvertToResourceName(_resourcesRootName)}.";
            }
        }

        public IStringLocalizer Create(Type source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return GetOrCreateLocalizer(source.Assembly, source.FullName);
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
            return _localizerCache.GetOrAdd(resourceRelativeName, key => new ResourceManagerStringLocalizer(assembly, key));
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