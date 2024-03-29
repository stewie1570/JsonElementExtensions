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
        public JsonValueKind ValueKind { get; set; }
    }

    public class JsonValue
    {
        public object Value { get; set; }
        public JsonValueKind ValueKind { get; set; }

        public static JsonValue Undefined => new JsonValue
        {
            Value = null,
            ValueKind = JsonValueKind.Undefined
        };
    }

    public static class JsonElementPathsQueryExtensions
    {
        public static Dictionary<string, JsonValue> PathsAndValuesDictionary(this JsonElement element, string prefix = "")
        {
            return PathsAndValues(element, prefix).ToDictionary();
        }

        public static Dictionary<string, Tuple<JsonValue, JsonValue>> DiffWith(this JsonElement element, JsonElement other)
        {
            return element.PathsAndValuesDictionary().DiffWith(other.PathsAndValuesDictionary());
        }

        public static Dictionary<string, Tuple<JsonValue, JsonValue>> DiffWith(this Dictionary<string, JsonValue> dict1, Dictionary<string, JsonValue> dict2)
        {
            var disctinctKeys = dict1.Keys.Concat(dict2.Keys).Distinct();
            return disctinctKeys
                .Where(key => dict1.ContainsKey(key) != dict2.ContainsKey(key)
                    || !dict1[key].Value.Equals(dict2[key].Value))
                .ToDictionary(
                    key => key,
                    key => Tuple.Create(
                        dict1.ContainsKey(key) ? dict1[key] : JsonValue.Undefined,
                        dict2.ContainsKey(key) ? dict2[key] : JsonValue.Undefined
                    )
                );
        }

        #region Helpers

        private static List<PathAndValue> PathsAndValues(this JsonElement element, string prefix = "")
        {
            return IsIterable(element)
                ? PropsFrom(element).SelectMany(AllSubPathsWith(prefix)).ToList()
                : new List<PathAndValue> {
                    new PathAndValue
                    {
                        Path = prefix,
                        Value = element.ToValue(),
                        ValueKind = element.ValueKind
                    }
                };
        }

        private static Dictionary<string, JsonValue> ToDictionary(this IEnumerable<PathAndValue> pathAndValues)
        {
            return pathAndValues.ToDictionary(p => p.Path, p => new JsonValue
            {
                Value = p.Value,
                ValueKind = p.ValueKind
            });
        }

        private class Property
        {
            public string Name { get; set; }
            public JsonElement JsonValue { get; set; }
        }

        private static Func<string, Func<Property, IEnumerable<PathAndValue>>> AllSubPathsWith =
            (string prefix) =>
            (Property prop) => prop
                .JsonValue
                .PathsAndValues(prefix: prop.Name)
                .Select(subPath => new PathAndValue
                {
                    Path = subPath.Path.PrefixWith(prefix),
                    Value = subPath.Value,
                    ValueKind = subPath.ValueKind
                });

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