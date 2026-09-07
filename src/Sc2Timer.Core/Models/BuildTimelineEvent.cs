namespace Sc2Timer.Core.Models;

/// <summary>
/// One concrete, timed occurrence in the expanded timeline (a single event, or one
/// instance of a recurring event). This is what the UI displays and the scheduler fires cues from.
/// </summary>
public sealed class BuildTimelineEvent
{
    public required TimeSpan Time { get; init; }
    public required string Label { get; init; }
    public required string Speech { get; init; }

    /// <summary>0 means no warning cue for this event.</summary>
    public double WarningSeconds { get; init; }
    public required string WarningSpeech { get; init; }
}
