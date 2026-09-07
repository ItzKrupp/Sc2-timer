using System.Diagnostics;

namespace Sc2Timer.Core;

public enum ClockState
{
    Stopped,
    Running,
    Paused,
}

/// <summary>
/// A simple start/pause/reset stopwatch. The real-time source is injectable so it can be
/// unit tested deterministically; the app uses the default (a real <see cref="Stopwatch"/>).
/// </summary>
public sealed class GameClock
{
    private readonly Func<TimeSpan> _now;
    private TimeSpan _accumulated = TimeSpan.Zero;
    private TimeSpan _runStartedAt;

    public GameClock(Func<TimeSpan>? nowProvider = null)
    {
        _now = nowProvider ?? DefaultNow;
    }

    public ClockState State { get; private set; } = ClockState.Stopped;

    public TimeSpan Elapsed => State == ClockState.Running
        ? _accumulated + (_now() - _runStartedAt)
        : _accumulated;

    public void Start()
    {
        if (State == ClockState.Running)
        {
            return;
        }

        _runStartedAt = _now();
        State = ClockState.Running;
    }

    public void Pause()
    {
        if (State != ClockState.Running)
        {
            return;
        }

        _accumulated += _now() - _runStartedAt;
        State = ClockState.Paused;
    }

    public void Reset()
    {
        _accumulated = TimeSpan.Zero;
        State = ClockState.Stopped;
    }

    private static readonly Stopwatch SharedStopwatch = Stopwatch.StartNew();
    private static TimeSpan DefaultNow() => SharedStopwatch.Elapsed;
}
