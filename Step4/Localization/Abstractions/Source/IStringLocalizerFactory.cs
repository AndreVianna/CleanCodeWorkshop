using System;

namespace TrdP.Localization.Abstractions
{
    public interface IStringLocalizerFactory
    {
        IStringLocalizer Create(Type resourceSource);

        IStringLocalizer Create(string baseName, string location);
    }
}