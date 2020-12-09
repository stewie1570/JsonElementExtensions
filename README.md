# JsonElementExtensions

## Purpose ##
*(expressed via a unit test*:

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
        .PathsAndValues()
        .Should()
        .BeEquivalentTo(new List<PathAndValue>
        {
            new PathAndValue{ Path = "prop1.prop2", Value = "value" },
            new PathAndValue{ Path = "contacts.0.info.name", Value = "Stewie" },
            new PathAndValue{ Path = "contacts.1.info.number", Value = 12 },
            new PathAndValue{ Path = "contacts.2.info.isAwesome", Value = true }
        });
}
```
