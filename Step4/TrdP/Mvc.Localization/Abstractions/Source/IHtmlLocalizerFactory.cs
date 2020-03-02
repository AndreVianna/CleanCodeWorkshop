namespace TrdP.Mvc.Localization.Abstractions
{
    public interface IHtmlLocalizerFactory
    {
        IHtmlLocalizer Create<TResourcesLocator>()
            where TResourcesLocator : class;

        IHtmlLocalizer Create(string resourcesRelativePath);
    }
}