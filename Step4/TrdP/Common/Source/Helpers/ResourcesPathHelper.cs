using System;
using System.IO;

namespace TrdP.Common.Helpers
{
    public static class ResourcesPathHelper
    {
        public static string GetReourcesLocatorRelativePath<TResourcesLocator>()
        {
            return GetReourcesLocatorRelativePath(typeof(TResourcesLocator));
        }

        public static string GetReourcesLocatorRelativePath(Type resourcesLocator)
        {
            return GetReourcesLocatorRelativePath(resourcesLocator.FullName, resourcesLocator.Assembly.GetName().Name);
        }

        public static string GetReourcesLocatorRelativePath(string targetName, string prefix = null)
        {
            var result = targetName
                .Trim()
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
            if (prefix != null && result.StartsWith(prefix))
            {
                result = result.Remove(0, prefix.Length);
            }
            return result.TrimStart('.');
        }
    }
}