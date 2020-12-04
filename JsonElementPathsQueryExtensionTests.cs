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
                .Paths()
                .Should()
                .BeEquivalentTo(new List<string> { });
        }

        [Fact]
        public void FindsShallowPaths()
        {
            JsonDocument
                .Parse("{ \"prop1\": \"value\", \"prop2\": \"value\" }")
                .RootElement
                .Paths()
                .Should()
                .BeEquivalentTo(new List<string>
                {
                    "prop1",
                    "prop2"
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
                .Paths()
                .Should()
                .BeEquivalentTo(new List<string>
                {
                    "prop1.prop2",
                    "contacts.info.name"
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
                .Paths()
                .Should()
                .BeEquivalentTo(new List<string>
                {
                    "prop1.prop2",
                    "contacts.0.info.name",
                    "contacts.1.info.name"
                });
        }
    }
}