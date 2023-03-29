# JsonElementExtensions

## Purpose ##
*(expressed via a unit test)*:

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
```
