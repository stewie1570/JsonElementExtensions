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
                .Parse("\"value\"")
                .RootElement
                .PathsAndValues()
                .Should()
                .BeEquivalentTo(new List<PathAndValue>
                {
                    new PathAndValue
                    {
                        Path = "",
                        Value = "value",
                        ValueKind = JsonValueKind.String
                    }
                });
        }

        [Fact]
        public void FindsShallowPaths()
        {
            JsonDocument
                .Parse("{ \"prop1\": \"value1\", \"prop2\": \"value2\" }")
                .RootElement
                .PathsAndValues()
                .Should()
                .BeEquivalentTo(new List<PathAndValue>
                {
                    new PathAndValue{ Path = "prop1", Value = "value1", ValueKind = JsonValueKind.String },
                    new PathAndValue{ Path = "prop2", Value = "value2", ValueKind = JsonValueKind.String }
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
                .PathsAndValues()
                .Should()
                .BeEquivalentTo(new List<PathAndValue>
                {
                    new PathAndValue{ Path = "prop1.prop2", Value = "value", ValueKind = JsonValueKind.String },
                    new PathAndValue{ Path = "contacts.info.name", Value = "Stewie", ValueKind = JsonValueKind.String }
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
                .PathsAndValues()
                .Should()
                .BeEquivalentTo(new List<PathAndValue>
                {
                    new PathAndValue{ Path = "prop1.prop2", Value = "value", ValueKind = JsonValueKind.String },
                    new PathAndValue{ Path = "contacts.0.info.name", Value = "Stewie", ValueKind = JsonValueKind.String },
                    new PathAndValue{ Path = "contacts.1.info.number", Value = 12, ValueKind = JsonValueKind.Number },
                    new PathAndValue{ Path = "contacts.2.info.isAwesome", Value = true, ValueKind = JsonValueKind.True }
                });
        }
    }
}