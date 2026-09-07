using Sc2Timer.Core.Models;

namespace Sc2Timer.Core;

/// <summary>Expands an authored <see cref="BuildOrderFile"/> into a flat, time-sorted timeline.</summary>
public static class TimelineBuilder
{
    /// <summary>If a recurring event has no endTime, it repeats for this long after its startTime.</summary>
    private static readonly TimeSpan DefaultRecurringWindow = TimeSpan.FromMinutes(20);

    public static List<BuildTimelineEvent> Build(BuildOrderFile file)
    {
        var result = new List<BuildTimelineEvent>();

        foreach (var dto in file.Events)
        {
            var label = dto.Text;
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new InvalidBuildOrderException("An event is missing its 'text'.");
            }

            var warningSeconds = dto.WarningSeconds ?? (dto.Recurring ? 0 : file.DefaultWarningSeconds);

            if (dto.Recurring)
            {
                if (string.IsNullOrWhiteSpace(dto.StartTime))
                {
                    throw new InvalidBuildOrderException($"Recurring event '{label}' is missing 'startTime'.");
                }

                if (dto.IntervalSeconds is null || dto.IntervalSeconds <= 0)
                {
                    throw new InvalidBuildOrderException($"Recurring event '{label}' needs a positive 'intervalSeconds'.");
                }

                var start = GameTime.Parse(dto.StartTime);
                var end = dto.EndTime is not null ? GameTime.Parse(dto.EndTime) : start + DefaultRecurringWindow;
                var interval = TimeSpan.FromSeconds(dto.IntervalSeconds.Value);

                for (var time = start; time <= end; time += interval)
                {
                    result.Add(MakeEvent(time, label, dto, warningSeconds));
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.Time))
                {
                    throw new InvalidBuildOrderException($"Event '{label}' is missing a 'time'.");
                }

                result.Add(MakeEvent(GameTime.Parse(dto.Time), label, dto, warningSeconds));
            }
        }

        result.Sort((a, b) => a.Time.CompareTo(b.Time));
        return result;
    }

    private static BuildTimelineEvent MakeEvent(TimeSpan time, string label, BuildEventDto dto, double warningSeconds)
    {
        return new BuildTimelineEvent
        {
            Time = time,
            Label = label,
            Speech = label,
            WarningSeconds = warningSeconds,
            WarningSpeech = dto.WarningText ?? $"{(int)Math.Round(warningSeconds)} seconds until {label}",
        };
    }
}
