namespace Sc2Timer.Core.Models;

/// <summary>The raw shape of a build order JSON file, as authored (e.g. by an AI assistant).</summary>
public sealed class BuildOrderFile
{
    public string Name { get; set; } = "";
    public string? Matchup { get; set; }

    /// <summary>Default warning lead time, in seconds, for non-recurring events that don't specify their own.</summary>
    public double DefaultWarningSeconds { get; set; } = 10;

    public List<BuildEventDto> Events { get; set; } = new();
}

/// <summary>One authored event: either a single timed cue, or a recurring cue (e.g. injects).</summary>
public sealed class BuildEventDto
{
    /// <summary>Spoken and displayed text for the main cue, e.g. "Start Lair".</summary>
    public string? Text { get; set; }

    /// <summary>"mm:ss" time for a single (non-recurring) event.</summary>
    public string? Time { get; set; }

    /// <summary>Overrides the build order's default warning lead time. 0 disables the warning for this event.</summary>
    public double? WarningSeconds { get; set; }

    /// <summary>Custom spoken text for the warning cue. Defaults to "{N} seconds until {text}".</summary>
    public string? WarningText { get; set; }

    /// <summary>Set true for a repeating event such as larva injects.</summary>
    public bool Recurring { get; set; }

    /// <summary>"mm:ss" time of the first occurrence. Required when Recurring is true.</summary>
    public string? StartTime { get; set; }

    /// <summary>Seconds between occurrences. Required when Recurring is true.</summary>
    public double? IntervalSeconds { get; set; }

    /// <summary>"mm:ss" time to stop repeating. Optional; defaults to 20 minutes after StartTime.</summary>
    public string? EndTime { get; set; }
}
