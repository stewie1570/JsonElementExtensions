using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace dynamic_iteration
{
    public static class JsonElementPathsQueryExtensions
    {
        public static List<string> Paths(this JsonElement element, string prefix = "")
        {
            return IsIterable(element)
                ? PropsFrom(element).SelectMany(AllSubPathsWith(prefix)).ToList()
                : new List<string> { };
        }

        #region Helpers

        private class Property
        {
            public string Name { get; set; }
            public JsonElement Value { get; set; }
        }

        private static Func<string, Func<Property, IEnumerable<string>>> AllSubPathsWith =
            (string prefix) =>
            (Property prop) => IsIterable(prop.Value)
                ? prop
                    .Value
                    .Paths(prefix: prop.Name)
                    .Select(subPath => subPath.PrefixWith(prefix))
                : new List<string> { prop.Name.PrefixWith(prefix) };

        private static IEnumerable<Property> PropsFrom(JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Array
                ? PropsFromJsonArray(element)
                : PropsFromJsonObject(element);
        }

        private static IEnumerable<Property> PropsFromJsonArray(JsonElement element)
        {
            return Enumerable
                .Range(0, element.GetArrayLength())
                .Select(index => new Property
                {
                    Name = index.ToString(),
                    Value = element[index]
                });
        }

        private static IEnumerable<Property> PropsFromJsonObject(JsonElement element)
        {
            return element
                .EnumerateObject()
                .Select(prop => new Property
                {
                    Name = prop.Name,
                    Value = prop.Value
                });
        }

        private static bool IsIterable(JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Object
                || element.ValueKind == JsonValueKind.Array;
        }

        private static string PrefixWith(this string subPath, string prefix)
        {
            return prefix == string.Empty ? subPath : $"{prefix}.{subPath}";
        }

        #endregion
    }
}