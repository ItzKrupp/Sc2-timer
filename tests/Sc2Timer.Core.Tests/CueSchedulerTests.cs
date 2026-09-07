using Sc2Timer.Core;
using Sc2Timer.Core.Models;
using Xunit;

namespace Sc2Timer.Core.Tests;

public class CueSchedulerTests
{
    private static BuildTimelineEvent Ev(string time, string label, double warningSeconds = 0) => new()
    {
        Time = GameTime.Parse(time),
        Label = label,
        Speech = label,
        WarningSeconds = warningSeconds,
        WarningSpeech = $"{(int)warningSeconds} seconds until {label}",
    };

    [Fact]
    public void FiresMainCueOnceElapsedReachesEventTime()
    {
        var scheduler = new CueScheduler(new[] { Ev("00:10", "Pool") });

        Assert.Empty(scheduler.Poll(TimeSpan.FromSeconds(9)));
        var fired = scheduler.Poll(TimeSpan.FromSeconds(10));

        var cue = Assert.Single(fired);
        Assert.Equal(CueKind.Main, cue.Kind);
        Assert.Equal("Pool", cue.Speech);

        // Doesn't fire again on a later poll.
        Assert.Empty(scheduler.Poll(TimeSpan.FromSeconds(11)));
    }

    [Fact]
    public void FiresWarningBeforeMainCue()
    {
        var scheduler = new CueScheduler(new[] { Ev("00:20", "Lair", warningSeconds: 10) });

        var atWarning = scheduler.Poll(TimeSpan.FromSeconds(10));
        Assert.Equal(CueKind.Warning, Assert.Single(atWarning).Kind);

        var atMain = scheduler.Poll(TimeSpan.FromSeconds(20));
        Assert.Equal(CueKind.Main, Assert.Single(atMain).Kind);
    }

    [Fact]
    public void SkipsWarningIfItWouldFallBeforeGameStart()
    {
        var scheduler = new CueScheduler(new[] { Ev("00:05", "Early", warningSeconds: 10) });

        // Warning would be at t=-5s, so it should never fire; only the main cue does.
        var fired = scheduler.Poll(TimeSpan.FromSeconds(5));

        var cue = Assert.Single(fired);
        Assert.Equal(CueKind.Main, cue.Kind);
    }

    [Fact]
    public void FiresBothCuesInOneCatchUpPollIfATickWasMissed()
    {
        var scheduler = new CueScheduler(new[] { Ev("00:20", "Lair", warningSeconds: 10) });

        var fired = scheduler.Poll(TimeSpan.FromSeconds(25));

        Assert.Equal(2, fired.Count);
        Assert.Contains(fired, c => c.Kind == CueKind.Warning);
        Assert.Contains(fired, c => c.Kind == CueKind.Main);
    }

    [Fact]
    public void ResetAllowsCuesToFireAgain()
    {
        var scheduler = new CueScheduler(new[] { Ev("00:10", "Pool") });
        scheduler.Poll(TimeSpan.FromSeconds(10));

        scheduler.Reset();

        Assert.Single(scheduler.Poll(TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public void GetNextAndUpcomingReflectElapsedTime()
    {
        var scheduler = new CueScheduler(new[] { Ev("00:10", "A"), Ev("00:20", "B"), Ev("00:30", "C") });

        Assert.Equal("A", scheduler.GetNext(TimeSpan.FromSeconds(5))!.Label);
        Assert.Equal(new[] { "B", "C" }, scheduler.GetUpcoming(TimeSpan.FromSeconds(10), 5).Select(e => e.Label));
        Assert.Equal(new[] { "A" }, scheduler.GetCompleted(TimeSpan.FromSeconds(10)).Select(e => e.Label));
        Assert.Null(scheduler.GetNext(TimeSpan.FromSeconds(30)));
    }
}
