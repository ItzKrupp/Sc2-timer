using Sc2Timer.Core;
using Xunit;

namespace Sc2Timer.Core.Tests;

public class BuildOrderLoaderTests
{
    [Fact]
    public void ParsesAWellFormedBuildOrder()
    {
        const string json = """
        {
          "name": "Test Build",
          "matchup": "ZvT",
          "defaultWarningSeconds": 10,
          "events": [
            { "time": "02:45", "text": "Start Lair" },
            { "recurring": true, "text": "Inject", "startTime": "02:30", "intervalSeconds": 29, "endTime": "04:00" }
          ]
        }
        """;

        var file = BuildOrderLoader.LoadFromJson(json);

        Assert.Equal("Test Build", file.Name);
        Assert.Equal("ZvT", file.Matchup);
        Assert.Equal(2, file.Events.Count);
    }

    [Fact]
    public void RejectsMissingName()
    {
        const string json = """{ "events": [{ "time": "00:10", "text": "x" }] }""";

        Assert.Throws<InvalidBuildOrderException>(() => BuildOrderLoader.LoadFromJson(json));
    }

    [Fact]
    public void RejectsNoEvents()
    {
        const string json = """{ "name": "Empty" }""";

        Assert.Throws<InvalidBuildOrderException>(() => BuildOrderLoader.LoadFromJson(json));
    }

    [Fact]
    public void RejectsMalformedJson()
    {
        Assert.Throws<InvalidBuildOrderException>(() => BuildOrderLoader.LoadFromJson("{ not json"));
    }
}
