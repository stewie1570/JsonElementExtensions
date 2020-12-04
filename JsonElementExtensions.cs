using System.Text.Json;

namespace dynamic_iteration
{
    public static class JsonElementExtensions
    {
        public static object At(this JsonElement element, string path)
        {
            return element.ValueKind == JsonValueKind.Undefined ? null
                : path.Contains(".") ? element.NextPropIn(path)
                : element.ValueFor(path);
        }

        #region Helpers

        private static object NextPropIn(this JsonElement element, string path)
        {
            var dotIndex = path.IndexOf('.');
            string currentPropName = path.Substring(0, dotIndex);
            string restOfPath = path.Substring(dotIndex + 1);

            return element.GetProp(currentPropName).At(restOfPath);
        }

        private static JsonElement GetProp(this JsonElement element, string currentPropName)
        {
            return element.ValueKind == JsonValueKind.Array
                ? element[int.Parse(currentPropName)]
                : PropNavigation(element, currentPropName);

            static JsonElement PropNavigation(JsonElement element, string currentPropName)
            {
                element.TryGetProperty(currentPropName, out var result);
                return result;
            }
        }

        private static object ValueFor(this JsonElement element, string path)
        {
            var result = element.GetProp(path);
            switch (result.ValueKind)
            {
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return result.GetBoolean();
                case JsonValueKind.Number:
                    return result.GetDouble();
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.String:
                    return result.GetString();
                default:
                    return null;
            }
        }

        #endregion
    }
}
