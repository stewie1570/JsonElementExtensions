using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace dynamic_iteration
{
    public class PathsExtensionsTests
    {
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
    }
}