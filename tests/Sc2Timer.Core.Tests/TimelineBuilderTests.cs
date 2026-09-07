using Sc2Timer.Core;
using Sc2Timer.Core.Models;
using Xunit;

namespace Sc2Timer.Core.Tests;

public class TimelineBuilderTests
{
    [Fact]
    public void ExpandsRecurringInjectsAtTheConfiguredInterval()
    {
        var file = new BuildOrderFile
        {
            Name = "Test",
            Events =
            {
                new BuildEventDto
                {
                    Text = "Inject",
                    Recurring = true,
                    StartTime = "02:30",
                    IntervalSeconds = 29,
                    EndTime = "04:00",
                },
            },
        };

        var timeline = TimelineBuilder.Build(file);

        Assert.Equal(
            new[] { "02:30", "02:59", "03:28", "03:57" },
            timeline.Select(e => GameTime.Format(e.Time)));
    }

    [Fact]
    public void SingleEventsUseTheDefaultWarningUnlessOverridden()
    {
        var file = new BuildOrderFile
        {
            Name = "Test",
            DefaultWarningSeconds = 10,
            Events =
            {
                new BuildEventDto { Text = "Start Lair", Time = "02:45" },
                new BuildEventDto { Text = "No warning here", Time = "03:00", WarningSeconds = 0 },
            },
        };

        var timeline = TimelineBuilder.Build(file);

        Assert.Equal(10, timeline[0].WarningSeconds);
        Assert.Equal("10 seconds until Start Lair", timeline[0].WarningSpeech);
        Assert.Equal(0, timeline[1].WarningSeconds);
    }

    [Fact]
    public void RecurringEventsDefaultToNoWarning()
    {
        var file = new BuildOrderFile
        {
            Name = "Test",
            Events =
            {
                new BuildEventDto
                {
                    Text = "Inject",
                    Recurring = true,
                    StartTime = "02:30",
                    IntervalSeconds = 29,
                    EndTime = "02:30",
                },
            },
        };

        var timeline = TimelineBuilder.Build(file);

        Assert.Equal(0, timeline[0].WarningSeconds);
    }

    [Fact]
    public void ThrowsWhenRecurringEventIsMissingRequiredFields()
    {
        var file = new BuildOrderFile
        {
            Name = "Test",
            Events = { new BuildEventDto { Text = "Inject", Recurring = true } },
        };

        Assert.Throws<InvalidBuildOrderException>(() => TimelineBuilder.Build(file));
    }

    [Fact]
    public void ResultIsSortedByTimeEvenWhenAuthoredOutOfOrder()
    {
        var file = new BuildOrderFile
        {
            Name = "Test",
            Events =
            {
                new BuildEventDto { Text = "Second", Time = "02:00" },
                new BuildEventDto { Text = "First", Time = "01:00" },
            },
        };

        var timeline = TimelineBuilder.Build(file);

        Assert.Equal(new[] { "First", "Second" }, timeline.Select(e => e.Label));
    }
}
