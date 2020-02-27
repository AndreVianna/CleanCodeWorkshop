using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace TrdP.ResourceManagerStringProvider
{
    public class ResourceManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly string _resourcesRootName;

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

            var assembly = source.Assembly;
            var assemblyName = new AssemblyName(assembly.FullName).Name;
            var sourceRelativeName = TrimPrefix(source.FullName, $"{assemblyName}.");
            var resourceRelativeName = $"{_resourcesRootName}{sourceRelativeName}";


            //return _localizerCache.GetOrAdd(baseName, _ =>
            //{
            return new ResourceManagerStringLocalizer(assembly, resourceRelativeName);
            //});
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
            sourceName = ConvertToResourceName(sourceName);
            var sourceRelativeName = TrimPrefix(sourceName, $"{assemblyName}.");
            var resourceRelativeName = $"{_resourcesRootName}{sourceRelativeName}";
            return new ResourceManagerStringLocalizer(assembly, resourceRelativeName);
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