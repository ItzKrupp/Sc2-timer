# SC2 Zerg Build Timer

An audio-based build-order timer for StarCraft 2 Zerg players. Import a build
order, press Start when the game begins, and it calls out timings ("Start
Lair", "Inject", "Third hatchery"...) so you can keep your eyes on the game.

## Why this tech stack

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
src/Sc2Timer.Core/    Build order parsing, timeline expansion, clock, cue scheduling (plain .NET, no UI)
src/Sc2Timer.App/     WPF UI + SAPI text-to-speech
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
