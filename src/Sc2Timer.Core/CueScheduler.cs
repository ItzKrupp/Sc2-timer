using Sc2Timer.Core.Models;

namespace Sc2Timer.Core;

public enum CueKind
{
    Warning,
    Main,
}

public sealed record CueFired(BuildTimelineEvent Event, CueKind Kind, string Speech);

/// <summary>
/// Given a time-sorted timeline, tells you which cues (warning / main) have newly become due
/// each time you poll it with the current elapsed game time. Pure logic, no clock of its own.
/// </summary>
public sealed class CueScheduler
{
    private readonly List<BuildTimelineEvent> _events;
    private readonly bool[] _warningFired;
    private readonly bool[] _mainFired;

    public CueScheduler(IEnumerable<BuildTimelineEvent> events)
    {
        _events = events.OrderBy(e => e.Time).ToList();
        _warningFired = new bool[_events.Count];
        _mainFired = new bool[_events.Count];
    }

    public IReadOnlyList<BuildTimelineEvent> Events => _events;

    /// <summary>Call periodically with the current elapsed time; returns any cues newly due since the last call.</summary>
    public List<CueFired> Poll(TimeSpan elapsed)
    {
        var fired = new List<CueFired>();

        for (var i = 0; i < _events.Count; i++)
        {
            var ev = _events[i];

            if (!_warningFired[i] && ev.WarningSeconds > 0)
            {
                var warnAt = ev.Time - TimeSpan.FromSeconds(ev.WarningSeconds);
                if (warnAt >= TimeSpan.Zero && elapsed >= warnAt)
                {
                    _warningFired[i] = true;
                    fired.Add(new CueFired(ev, CueKind.Warning, ev.WarningSpeech));
                }
            }

            if (!_mainFired[i] && elapsed >= ev.Time)
            {
                _mainFired[i] = true;
                fired.Add(new CueFired(ev, CueKind.Main, ev.Speech));
            }
        }

        return fired;
    }

    public void Reset()
    {
        Array.Clear(_warningFired);
        Array.Clear(_mainFired);
    }

    public BuildTimelineEvent? GetNext(TimeSpan elapsed) => _events.FirstOrDefault(e => e.Time > elapsed);

    public IEnumerable<BuildTimelineEvent> GetUpcoming(TimeSpan elapsed, int count) =>
        _events.Where(e => e.Time > elapsed).Take(count);

    public IEnumerable<BuildTimelineEvent> GetCompleted(TimeSpan elapsed) =>
        _events.Where(e => e.Time <= elapsed);
}
