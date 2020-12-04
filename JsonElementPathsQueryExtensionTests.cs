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
                .BeEquivalentTo(new List<PathAndValue> { });
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
                    new PathAndValue{ Path = "prop1", Value = "value1" },
                    new PathAndValue{ Path = "prop2", Value = "value2" }
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
                    new PathAndValue{ Path = "prop1.prop2", Value = "value" },
                    new PathAndValue{ Path = "contacts.info.name", Value = "Stewie" }
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
                        { ""info"": { ""name"": ""John"" } }
                    ]
                }")
                .RootElement
                .PathsAndValues()
                .Should()
                .BeEquivalentTo(new List<PathAndValue>
                {
                    new PathAndValue{ Path = "prop1.prop2", Value = "value" },
                    new PathAndValue{ Path = "contacts.0.info.name", Value = "Stewie" },
                    new PathAndValue{ Path = "contacts.1.info.name", Value = "John" }
                });
        }
    }
}