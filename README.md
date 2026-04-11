# PomodoroTimer

[中文说明 / Chinese documentation](README.zh-CN.md)

PomodoroTimer is a .NET 8 desktop Pomodoro timer built with Avalonia. The current app presents a Chinese-language interface for running a simple focus/break loop with start, pause, reset, and phase-skip controls.

## Overview

- Focus session: 25 minutes
- Short break: 5 minutes
- Controls for start/pause, reset, and skipping to the next phase
- Progress tracking for the active phase and completed focus sessions
- MVVM-based Avalonia UI backed by unit-tested timer state logic

## Tech Stack

- .NET 8
- Avalonia 12
- CommunityToolkit.Mvvm
- xUnit

## Repository Layout

```text
.
|-- PomodoroTimer.sln
|-- PomodoroTimer/
|   |-- Program.cs
|   |-- App.axaml
|   |-- Views/MainWindow.axaml
|   |-- ViewModels/MainWindowViewModel.cs
|   `-- Models/PomodoroTimerState.cs
`-- PomodoroTimer.Tests/
    |-- PomodoroTimer.Tests.csproj
    `-- PomodoroTimerStateTests.cs
```

Key files:

- `PomodoroTimer/Views/MainWindow.axaml`: main desktop layout, controls, and visual styling
- `PomodoroTimer/ViewModels/MainWindowViewModel.cs`: UI-facing labels, commands, and progress calculations
- `PomodoroTimer/Models/PomodoroTimerState.cs`: focus/break state machine and status messaging
- `PomodoroTimer.Tests/PomodoroTimerStateTests.cs`: timer-state regression coverage

## Getting Started

### Prerequisite

- .NET 8 SDK

### Build

```bash
dotnet build PomodoroTimer.sln
```

### Run

```bash
dotnet run --project PomodoroTimer/PomodoroTimer.csproj
```

### Test

```bash
dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --no-build
```

## Current App Behavior

- The app starts in a 25-minute focus session
- Completing a focus session switches to a 5-minute short break
- Completing a break switches back to focus mode
- Manually skipping a phase does not increment the completed-focus counter
- The current UI copy is written in Chinese

## Chinese Version

For a Chinese project introduction and setup guide, see [README.zh-CN.md](README.zh-CN.md).

