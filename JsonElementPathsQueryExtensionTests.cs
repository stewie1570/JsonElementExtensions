using System;
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

        [Fact]
        public void DiffsTwoJsonDocuments()
        {
            var left = JsonDocument.Parse(@"{
                ""prop1"": { ""prop2"": 1 },
                ""contacts"": [
                    { ""info"": { ""name"": ""Stewie"" } },
                    { ""info"": { ""number"": 12 } },
                    { ""info"": { ""isAwesome"": true } }
                ]
            }");

            var right = JsonDocument.Parse(@"{
                ""prop1"": { ""prop2"": ""value2"" },
                ""contacts"": [
                    { ""info"": { ""name"": ""Stewie"" } },
                    { ""info"": { ""number"": 13 } },
                    { ""info"": { ""isAwesome"": false } },
                    { ""info"": { ""isSomething"": true } }
                ]
            }");

            left.RootElement.DiffWith(right.RootElement)
                .Should()
                .BeEquivalentTo(new Dictionary<string, Tuple<JsonValue, JsonValue>>
                {
                    ["prop1.prop2"] = new Tuple<JsonValue, JsonValue>(
                        new JsonValue { Value = 1, ValueKind = JsonValueKind.Number },
                        new JsonValue { Value = "value2", ValueKind = JsonValueKind.String }),
                    ["contacts.1.info.number"] = new Tuple<JsonValue, JsonValue>(
                        new JsonValue { Value = 12, ValueKind = JsonValueKind.Number },
                        new JsonValue { Value = 13, ValueKind = JsonValueKind.Number }),
                    ["contacts.2.info.isAwesome"] = new Tuple<JsonValue, JsonValue>(
                        new JsonValue { Value = true, ValueKind = JsonValueKind.True },
                        new JsonValue { Value = false, ValueKind = JsonValueKind.False }),
                    ["contacts.3.info.isSomething"] = new Tuple<JsonValue, JsonValue>(
                        new JsonValue { Value = null, ValueKind = JsonValueKind.Undefined },
                        new JsonValue { Value = true, ValueKind = JsonValueKind.True })
                });
        }
    }
}
