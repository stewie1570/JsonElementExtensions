using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace dynamic_iteration
{
    public class JsonElementExensionsPathQueryTests
    {
        [Fact]
        public void FindsNoPathsForValues()
        {
            JsonDocument
                .Parse(@"""value""")
                .RootElement
                .PathsAndValuesDictionary()
                .Should()
                .BeEquivalentTo(new Dictionary<string, JsonValue>
                {
                    [""] = new JsonValue { Value = "value", ValueKind = JsonValueKind.String }
                });
        }
        [Fact]
        public void FindsShallowPaths()
        {
            JsonDocument
                .Parse("{ \"prop1\": \"value1\", \"prop2\": \"value2\" }")
                .RootElement
                .PathsAndValuesDictionary()
                .Should()
                .BeEquivalentTo(new Dictionary<string, JsonValue>
                {
                    ["prop1"] = new JsonValue { Value = "value1", ValueKind = JsonValueKind.String },
                    ["prop2"] = new JsonValue { Value = "value2", ValueKind = JsonValueKind.String }
                });
        }

        [Fact]
        public void FindsDeepObjectPaths()
        {
            JsonDocument
                .Parse(@"{
                ""prop1"": { ""prop2"": ""value"" },
                ""contacts"": { ""info"": { ""name"": ""Stewie"" } }
            }")
                .RootElement
                .PathsAndValuesDictionary()
                .Should()
                .BeEquivalentTo(new Dictionary<string, JsonValue>
                {
                    ["prop1.prop2"] = new JsonValue { Value = "value", ValueKind = JsonValueKind.String },
                    ["contacts.info.name"] = new JsonValue
                    {
                        Value = "Stewie",
                        ValueKind = JsonValueKind.String
                    }
                });
        }

        [Fact]
        public void FindsDeepObjectAndArrayPaths()
        {
            JsonDocument
                .Parse(@"{
                ""prop1"": { ""prop2"": ""value"" },
                ""contacts"": [
                    { ""info"": { ""name"": ""Stewie"" } },
                    { ""info"": { ""number"": 12 } },
                    { ""info"": { ""isAwesome"": true } }
                ]
            }")
                .RootElement
                .PathsAndValuesDictionary()
                .Should()
                .BeEquivalentTo(new Dictionary<string, JsonValue>
                {
                    ["prop1.prop2"] = new JsonValue { Value = "value", ValueKind = JsonValueKind.String },
                    ["contacts.0.info.name"] = new JsonValue
                    {
                        Value = "Stewie",
                        ValueKind = JsonValueKind.String
                    },
                    ["contacts.1.info.number"] = new JsonValue
                    {
                        Value = 12,
                        ValueKind = JsonValueKind.Number
                    },
                    ["contacts.2.info.isAwesome"] = new JsonValue
                    {
                        Value = true,
                        ValueKind = JsonValueKind.True
                    }
                });
        }
    }
}
