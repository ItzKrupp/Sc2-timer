using Sc2Timer.Core;
using Xunit;

namespace Sc2Timer.Core.Tests;

public class GameClockTests
{
    private sealed class FakeTime
    {
        public TimeSpan Now { get; set; } = TimeSpan.Zero;
    }

    [Fact]
    public void ElapsedAdvancesOnlyWhileRunning()
    {
        var fake = new FakeTime();
        var clock = new GameClock(() => fake.Now);

        Assert.Equal(TimeSpan.Zero, clock.Elapsed);

        clock.Start();
        fake.Now = TimeSpan.FromSeconds(5);
        Assert.Equal(TimeSpan.FromSeconds(5), clock.Elapsed);

        clock.Pause();
        fake.Now = TimeSpan.FromSeconds(9999);
        Assert.Equal(TimeSpan.FromSeconds(5), clock.Elapsed);
    }

    [Fact]
    public void ResumingAfterPauseAccumulatesElapsedTime()
    {
        var fake = new FakeTime();
        var clock = new GameClock(() => fake.Now);

        clock.Start();
        fake.Now = TimeSpan.FromSeconds(5);
        clock.Pause();

        fake.Now = TimeSpan.FromSeconds(100); // time passes while paused, shouldn't count
        clock.Start();
        fake.Now = TimeSpan.FromSeconds(103);

        Assert.Equal(TimeSpan.FromSeconds(8), clock.Elapsed);
    }

    [Fact]
    public void ResetReturnsToZeroAndStopped()
    {
        var fake = new FakeTime();
        var clock = new GameClock(() => fake.Now);

        clock.Start();
        fake.Now = TimeSpan.FromSeconds(5);
        clock.Reset();

        Assert.Equal(TimeSpan.Zero, clock.Elapsed);
        Assert.Equal(ClockState.Stopped, clock.State);
    }
}
