﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using static TrdP.Common.Helpers.ResourcesPathHelper;

namespace TrdP.Localization
{
    public class StringLocalizerFactory<TProviderLocator> : IStringLocalizerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, IStringLocalizer> _localizerCache = new ConcurrentDictionary<string, IStringLocalizer>();

        public StringLocalizerFactory(IOptions<LocalizationProviderOptions> localizationOptions, ILoggerFactory loggerFactory)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            ProviderAssembly = typeof(TProviderLocator).Assembly;
            var resourcesRoot = (localizationOptions.Value.ResourcesRoot ?? string.Empty).Trim();
            ResourcesRoot = GetResourcesLocatorRelativePath(resourcesRoot);
        }

        internal Assembly ProviderAssembly { get; }

        internal string ResourcesRoot { get; }

        public IStringLocalizer Create<TResourcesLocator>()
            where TResourcesLocator : class
        {
            return Create(typeof(TResourcesLocator));
        }

        public IStringLocalizer Create(Type resourcesLocator)
        {
            return GetOrCreateLocalizer(GetResourcesLocatorRelativePath(resourcesLocator));
        }

        public IStringLocalizer Create(string resourcesRelativePath)
        {
            return GetOrCreateLocalizer(GetResourcesLocatorRelativePath(resourcesRelativePath));
        }

        public IStringLocalizer CreateForSharedResources()
        {
            return Create(typeof(TProviderLocator));
        }

        private IStringLocalizer GetOrCreateLocalizer(string targetRelativePath)
        {
            return _localizerCache.GetOrAdd(targetRelativePath, _ => new StringLocalizer(ProviderAssembly, ResourcesRoot, targetRelativePath, _loggerFactory.CreateLogger<StringLocalizer>()));
        }
    }
}