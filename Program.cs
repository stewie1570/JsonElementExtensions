using System;
using System.Linq;
using System.Text.Json;
using dynamic_iteration;

Func<string> ReadUnilEmpty = () =>
{
    var buffer = "";
    string input;

    while ((input = Console.ReadLine()) != "")
    {
        buffer += input;
    }

    return buffer;
};

Console.WriteLine("JSON: ");
var json = ReadUnilEmpty();

JsonDocument
    .Parse(json)
    .RootElement
    .PathsAndValuesDictionary()
    .OrderBy(value => value.Key)
    .Select(value => value.Key)
    .ToList()
    .ForEach(Console.WriteLine);