using System;

namespace TrdP.Localization.Mvc.Abstractions
{
    public interface IHtmlLocalizerFactory
    {
        IHtmlLocalizer Create(Type resourceSource);

        IHtmlLocalizer Create(string sourceName, string assemblyName);
    }
}