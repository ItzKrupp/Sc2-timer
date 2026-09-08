# SC2 Build Timer

An audio-based build-order timer for StarCraft 2, for any race. Set up (or
import) a build order, press Start when the game begins, and it calls out
timings ("Start Lair", "Inject", "MULE", "Chrono boost"...) so you can keep
your eyes on the game.

There are two versions in this repo:

- **`web/index.html`** (recommended) — a single self-contained HTML file.
  Double-click it to open it in any browser (Chrome/Edge work best on
  Windows) — no install, no build step, works fully offline. This is the
  actively-tested version.
- **`src/`** — a native .NET/WPF version. Written but not yet built or run
  (see the note near the bottom of this file) — kept here in case you want a
  "real" installed Windows app later.

## Web version (`web/index.html`)

Uses the browser's built-in Web Speech API for text-to-speech — no server,
no API key, no account, and no network access needed once the page is open.
Voices come from your OS/browser, same as the .NET version's approach but
with zero build step.

Just open `web/index.html` in a browser. It's not Zerg-specific — the JSON
format is just timestamps and labels, so it works equally well for Terran
MULEs or Protoss chrono boost (see `samples/` for one example per race).

**You don't need to prepare a JSON file to use it.** The **Load a build
order** section has a race selector (Zerg/Terran/Protoss) with 4 built-in
builds per race — pick one and go. Or use the click-through **Edit build
order** panel: give your build a name, then add events one at a time (a
time, a label, and optionally "repeats every N seconds" for things like
injects/MULEs/chrono). Or click **Import** to load a JSON file — one you
wrote by hand, or one an AI assistant generated for you (see
`docs/BUILD_ORDER_FORMAT.md`). Whatever's currently loaded is saved
automatically (browser local storage) so it's still there next time you open
the file, and **Download as JSON** lets you save a build you built in the
editor for reuse or sharing.

**A note on the built-in builds' accuracy.** StarCraft II got a major balance
patch (5.0.16, June 2026) that cut starting workers from 12 to 8 — probably
the biggest economy change the game has had in years, and it's still only a
couple of months old with two hotfixes since (one of which reverted a Ghost
supply change back to its original value, a reminder that even "current"
details from this patch are still moving). The built-in builds reflect the
right *structure* for it (correct building/tech order, and a handful of
concretely-confirmed numbers like the 8-worker start, the 2:30 third
hatchery, and Warpgate research moving onto the Gateway itself), but I
couldn't reach build-order databases like Spawning Tool from this
environment to pull verified replay timings, so exact seconds beyond what's
specifically confirmed are estimates. Each one shows a short note when
loaded saying as much — treat them as a solid starting shape, watch how they
actually play out, and adjust via **Edit build order**.

## Why the .NET/WPF version uses this stack

- **.NET 8 + WPF** for the UI — a small, native Windows desktop app with no
  extra runtime to install (a self-contained build is a single folder you can
  run on any Windows 10/11 machine).
- **System.Speech (SAPI)** for text-to-speech — this ships with Windows,
  works fully offline, and needs no API key or account. It uses whatever
  voices are already installed on your PC.
- **JSON build order files** — simple enough that ChatGPT or Claude can
  generate one for you (see `docs/BUILD_ORDER_FORMAT.md`).

The timing/scheduling logic lives in a plain **Sc2Timer.Core** library with no
UI or Windows dependencies. The idea is that a future version could feed it
elapsed time from something smarter (e.g. watching the SC2 replay log) instead
of a manual Start button, without having to touch the scheduling logic itself.

## Project layout

```
web/index.html        Self-contained HTML/JS version (recommended) — open directly in a browser
src/Sc2Timer.Core/    .NET version: build order parsing, timeline expansion, clock, cue scheduling
src/Sc2Timer.App/     .NET version: WPF UI + SAPI text-to-speech
tests/                xUnit tests for Sc2Timer.Core
samples/              Example build order JSON files
docs/                 Build order file format reference
```

## Requirements

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Running it

```
dotnet run --project src/Sc2Timer.App
```

Or open `Sc2Timer.sln` in Visual Studio 2022 and press F5.

The first build downloads the `System.Speech` NuGet package (a one-time,
official Microsoft package) — after that, the app itself makes no network
calls at all.

## Using it

1. Click **IMPORT** and pick a build order JSON file (try
   `samples/zerg-2base-roach-ravager.json` first).
2. Click **START** the moment your game begins.
3. Play — the app speaks each cue as it comes up, with a configurable warning
   a few seconds ahead of time.
4. **PAUSE**/**RESET** as needed (e.g. if a game doesn't start cleanly).

See `docs/BUILD_ORDER_FORMAT.md` for the JSON format and a ready-to-use prompt
for generating your own build orders with an AI assistant.

## Running the tests

```
dotnet test
```

These cover build order parsing, recurring-event expansion (e.g. injects
landing on the right cadence), and cue-firing logic — no Windows or UI
dependencies required.

## A note on this environment

This project was scaffolded in a Linux sandbox that can't run the WPF UI or
install the .NET SDK (both need Windows and are outside this sandbox's
network allowlist). The core logic in `Sc2Timer.Core` was verified with a
standalone script before being ported to C#, and the tests in
`tests/Sc2Timer.Core.Tests` cover it — but **the WPF app itself (`Sc2Timer.App`)
has not been built or run yet**. Please run `dotnet build`/`dotnet run` on
your Windows machine and let me know if anything doesn't compile or behave
as expected — XAML/WPF wiring mistakes are the most likely thing to have
slipped through.
