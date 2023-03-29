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
```
