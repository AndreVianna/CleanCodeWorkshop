using System;

namespace TrdP.Localization.Abstractions
{
    public sealed class NullStringLocalizerFactory : IStringLocalizerFactory
    {
        private NullStringLocalizerFactory()
        {
        }

        public static IStringLocalizerFactory Instance { get; } = new NullStringLocalizerFactory();

        public IStringLocalizer Create<TResourcesLocator>() where TResourcesLocator : class => NullStringLocalizer.Instance;
        public IStringLocalizer Create(Type resourcesLocator) => NullStringLocalizer.Instance;
        public IStringLocalizer Create(string resourcesRelativePath) => NullStringLocalizer.Instance;
        public IStringLocalizer CreateForSharedResources() => NullStringLocalizer.Instance;
    }
}