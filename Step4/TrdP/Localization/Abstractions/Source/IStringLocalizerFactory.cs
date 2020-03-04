using System;

namespace TrdP.Localization.Abstractions
{
    public interface IStringLocalizerFactory
    {
        IStringLocalizer Create<TResourcesLocator>() where TResourcesLocator : class;

        IStringLocalizer Create(Type resourcesLocator);

        IStringLocalizer Create(string resourcesRelativePath);

        IStringLocalizer CreateForSharedResources();
    }
}