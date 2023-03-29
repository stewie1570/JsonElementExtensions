using System.Collections.Generic;
using System.Linq;

namespace dynamic_iteration
{
    public static class PathsExtensions
    {
        public static bool IsSupportedBy(this string path, IEnumerable<string> supportedPathPatterns)
        {
            return supportedPathPatterns.Any(pattern => pattern.IsAPathMatchWith(path));
        }

        public static bool IsAPathMatchWith(this string pathPattern, string path)
        {
            var patternParts = pathPattern.Split('.');
            var pathParts = path.Split('.');

            return pathParts.Length == patternParts.Length
                && patternParts
                    .Zip(pathParts, (patternPart, pathPart) => new { patternPart, pathPart })
                    .All(_ => _.patternPart == "*" || _.patternPart == _.pathPart);
        }
    }
}