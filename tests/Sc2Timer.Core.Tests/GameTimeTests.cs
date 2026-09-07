using Sc2Timer.Core;
using Xunit;

namespace Sc2Timer.Core.Tests;

public class GameTimeTests
{
    [Theory]
    [InlineData("02:45", 165)]
    [InlineData("00:00", 0)]
    [InlineData("10:05", 605)]
    public void Parse_ReadsMinutesAndSeconds(string text, int expectedSeconds)
    {
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), GameTime.Parse(text));
    }

    [Fact]
    public void Parse_RejectsMalformedInput()
    {
        Assert.Throws<InvalidBuildOrderException>(() => GameTime.Parse("bad"));
    }

    [Theory]
    [InlineData(0, "00:00")]
    [InlineData(165, "02:45")]
    [InlineData(605, "10:05")]
    public void Format_ProducesMmSs(int seconds, string expected)
    {
        Assert.Equal(expected, GameTime.Format(TimeSpan.FromSeconds(seconds)));
    }
}
