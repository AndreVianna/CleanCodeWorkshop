using System;
using System.IO;

namespace TrdP.Common.Helpers
{
    public static class ResourcesPathHelper
    {
        public static string GetResourcesLocatorRelativePath(Type resourcesLocator)
        {
            return GetResourcesLocatorRelativePath(resourcesLocator.FullName, resourcesLocator.Assembly.GetName().Name);
        }

        public static string GetResourcesLocatorRelativePath(string targetName, string prefix = null)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return string.Empty;
            }

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