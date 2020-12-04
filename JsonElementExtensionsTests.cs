using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace dynamic_iteration
{
    public class JsonElementExensionsTests
    {
        [Fact]
        public void GetNumber()
        {
            JsonDocument
                .Parse("{ \"prop\": { \"value\": 12 } }")
                .RootElement
                .At("prop.value")
                .Should()
                .Be(12);
        }

        [Fact]
        public void GetString()
        {
            JsonDocument
                .Parse(@"{ ""contact"": { ""name"": { ""first"": ""stewie"" } } }")
                .RootElement
                .At("contact.name.first")
                .Should()
                .Be("stewie");
        }

        [Fact]
        public void GetValueInArray()
        {
            JsonDocument
                .Parse(@"{ ""contacts"": [
                    {""name"": ""tom""},
                    {""name"": ""stewie""},
                    {""name"": ""george""}
                ] }")
                .RootElement
                .At("contacts.1.name")
                .Should()
                .Be("stewie");
        }

        [Fact]
        public void GetBoolean()
        {
            JsonDocument
                .Parse(@"{ ""test"": { ""isAwesome"": true } }")
                .RootElement
                .At("test.isAwesome")
                .Should()
                .Be(true);
        }

        [Fact]
        public void GetNull()
        {
            JsonDocument
                .Parse(@"{ ""test"": { ""something"": null } }")
                .RootElement
                .At("test.something")
                .Should()
                .Be(null);
        }

        [Fact]
        public void NonExistantPathReturnsNull()
        {
            JsonDocument
                .Parse(@"{ ""test"": { } }")
                .RootElement
                .At("test.something.somethingElse.some.thing")
                .Should()
                .Be(null);
        }
    }
}
