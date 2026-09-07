namespace Sc2Timer.Core;

/// <summary>Parses and formats the "mm:ss" game-time notation used in build order files and the UI.</summary>
public static class GameTime
{
    public static TimeSpan Parse(string text)
    {
        var parts = text.Split(':');
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], out var minutes) ||
            !int.TryParse(parts[1], out var seconds))
        {
            throw new InvalidBuildOrderException($"Invalid time '{text}', expected mm:ss.");
        }

        return new TimeSpan(0, minutes, seconds);
    }

    public static string Format(TimeSpan time)
    {
        var totalSeconds = Math.Max(0, (int)time.TotalSeconds);
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }
}
