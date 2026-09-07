# Build order file format

A build order is a JSON file with this shape:

```jsonc
{
  "name": "Zerg 2-Base Roach/Ravager",   // shown in the UI
  "matchup": "ZvT",                       // optional, just informational
  "defaultWarningSeconds": 10,             // "10 seconds until X" lead time for normal events
  "events": [
    // A single timed event:
    { "time": "02:45", "text": "Start Lair" },

    // Same, but with its own warning lead time (0 disables the warning):
    { "time": "03:00", "text": "Third hatchery", "warningSeconds": 15 },

    // Same, but with custom wording for the warning cue:
    { "time": "03:30", "text": "Start plus one melee", "warningText": "Ten seconds until upgrade" },

    // A recurring event (e.g. larva injects), repeating at a fixed interval:
    {
      "recurring": true,
      "text": "Inject",
      "startTime": "02:30",
      "intervalSeconds": 29,
      "endTime": "12:00"          // optional; defaults to 20 minutes after startTime
    }
  ]
}
```

## Field reference

| Field | Applies to | Required | Notes |
|---|---|---|---|
| `name` | file | yes | Build order title, shown in the UI. |
| `matchup` | file | no | Free text (e.g. `"ZvT"`), informational only. |
| `defaultWarningSeconds` | file | no | Default warning lead time for non-recurring events. Default `10`. |
| `text` | event | yes | Spoken and displayed label, e.g. `"Start Lair"`. |
| `time` | single event | yes | `"mm:ss"` game time. |
| `warningSeconds` | event | no | Overrides the default. `0` disables the warning cue for this event. Recurring events default to `0` (no warning) unless set. |
| `warningText` | event | no | Custom spoken text for the warning cue. Defaults to `"{N} seconds until {text}"`. |
| `recurring` | event | no | Set `true` for a repeating event. |
| `startTime` | recurring event | yes | `"mm:ss"` of the first occurrence. |
| `intervalSeconds` | recurring event | yes | Seconds between occurrences. |
| `endTime` | recurring event | no | `"mm:ss"` to stop repeating. Defaults to 20 minutes after `startTime`. |

Times are always `"mm:ss"` (e.g. `"04:20"`, not `"4:20"` or `260`).

## Generating one with ChatGPT or Claude

Paste this prompt (edit the build description) into any chat AI:

> Create a StarCraft 2 build order for [describe the build, e.g. "Zerg 2-base
> Roach Ravager all-in vs Terran"]. Output it as JSON matching this schema:
> [paste the contents of this file, or the `zerg-2base-roach-ravager.json`
> sample]. Only include major milestones — tech buildings, upgrades,
> expansions, and a recurring "Inject" entry for larva injects. Don't include
> every single unit or supply block.

Save the AI's output as a `.json` file and use **Import** in the app to load it.
