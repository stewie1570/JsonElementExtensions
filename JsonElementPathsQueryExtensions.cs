using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace dynamic_iteration
{

    public class PathAndValue
    {
        public string Path { get; set; }
        public object Value { get; set; }
    }

    public static class JsonElementPathsQueryExtensions
    {
        public static List<PathAndValue> PathsAndValues(this JsonElement element, string prefix = "")
        {
            return IsIterable(element)
                ? PropsFrom(element).SelectMany(AllSubPathsWith(prefix)).ToList()
                : new List<PathAndValue> {
                    new PathAndValue
                    {
                        Path = string.Empty,
                        Value = element.ToValue()
                    }
                };
        }

        #region Helpers

        private class Property
        {
            public string Name { get; set; }
            public JsonElement JsonValue { get; set; }
        }

        private static Func<string, Func<Property, IEnumerable<PathAndValue>>> AllSubPathsWith =
            (string prefix) =>
            (Property prop) => IsIterable(prop.JsonValue)
                ? prop
                    .JsonValue
                    .PathsAndValues(prefix: prop.Name)
                    .Select(subPath => new PathAndValue
                    {
                        Path = subPath.Path.PrefixWith(prefix),
                        Value = subPath.Value
                    })
                : new List<PathAndValue> {
                    new PathAndValue {
                        Path = prop.Name.PrefixWith(prefix),
                        Value = prop.JsonValue.ToValue()
                    }
                };

        private static object ToValue(this JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Number:
                    return element.GetDouble();
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.String:
                    return element.GetString();
                default:
                    return null;
            }
        }

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
                    JsonValue = element[index]
                });
        }

        private static IEnumerable<Property> PropsFromJsonObject(JsonElement element)
        {
            return element
                .EnumerateObject()
                .Select(prop => new Property
                {
                    Name = prop.Name,
                    JsonValue = prop.Value
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