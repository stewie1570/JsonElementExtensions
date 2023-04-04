# JsonElementExtensions

## Purpose ##
*(expressed via tests)*:

```csharp
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
public void FilteringToKnownPaths()
{
    var objectValues = new List<PathAndValue>
    {
        new PathAndValue { Path = "prop1.prop2", Value = "value" },
        new PathAndValue { Path = "contacts.0.info.name", Value = "Stewie" },
        new PathAndValue { Path = "contacts.0.info.name.last", Value = "Anderson" },
        new PathAndValue { Path = "contacts.1.info.number", Value = 12 },
        new PathAndValue { Path = "contacts.2.info.isAwesome", Value = true }
    };

    var pathPatterns = new List<string>
    {
        "prop1.prop2",
        "contacts.*.info.name",
        "contacts.*.info.number"
    };

    objectValues
        .Where(value => value.Path.IsSupportedBy(pathPatterns))
        .Should()
        .BeEquivalentTo(new List<PathAndValue>
        {
            new PathAndValue { Path = "prop1.prop2", Value = "value" },
            new PathAndValue { Path = "contacts.0.info.name", Value = "Stewie" },
            new PathAndValue { Path = "contacts.1.info.number", Value = 12 }
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
```
