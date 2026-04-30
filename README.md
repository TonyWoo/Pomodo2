# Pomodo Timer

[中文说明 / Chinese documentation](README.zh-CN.md)

Pomodo Timer is a Pomodoro timer built with Avalonia. The repository shares one app layer across Windows and macOS desktop, plus dedicated iOS and Android heads that host the same timer UI, localization, settings, session history, and state logic.

## Overview

- Default focus session: 25 minutes
- Default short break: 5 minutes
- Presets for 25 / 5 and 50 / 5, plus custom work and break durations
- Topic entry before a focus session starts
- Start, pause, resume, and reset controls
- Drift-resistant timer calculations based on target end time
- Daily completed pomodoro count and focus-session records persisted to local JSON
- Simplified Chinese, Traditional Chinese, and English UI text
- One shared Avalonia view-model/model/service/localization stack across desktop and mobile heads

## Tech Stack

- .NET 8 for the shared app, desktop head, and tests
- .NET 10 mobile heads required by current Avalonia Android/iOS packages
- Avalonia 12
- CommunityToolkit.Mvvm
- xUnit

## Repository Layout

```text
.
|-- PomodoroTimer.sln
|-- PomodoroTimer.CrossPlatform.slnx
|-- PomodoroTimer/
|   |-- App.axaml
|   |-- Services/
|   |-- Styles/
|   |-- Views/MainView.axaml
|   |-- Views/TimerView.axaml
|   |-- Views/StatsView.axaml
|   |-- Views/SettingsView.axaml
|   |-- Views/MainWindow.axaml
|   |-- ViewModels/MainWindowViewModel.cs
|   `-- Models/PomodoroTimerState.cs
|-- PomodoroTimer.Desktop/
|   `-- Program.cs
|-- PomodoroTimer.Android/
|   `-- MainActivity.cs
|-- PomodoroTimer.iOS/
|   `-- AppDelegate.cs
`-- PomodoroTimer.Tests/
    `-- PomodoroTimer.Tests.csproj
```

Key files:

- `PomodoroTimer/App.axaml.cs`: shared Avalonia startup that routes desktop and mobile lifetimes into the same timer experience
- `PomodoroTimer/Views/MainView.axaml`: shared responsive shell and navigation used by desktop and mobile heads
- `PomodoroTimer/Views/TimerView.axaml`: timer page with circular progress, topic entry, controls, today count, and records
- `PomodoroTimer/Views/StatsView.axaml`: daily focus statistics and session records
- `PomodoroTimer/Views/SettingsView.axaml`: duration presets, custom durations, and language selection
- `PomodoroTimer/Services/`: timer, settings, session, and app-data service abstractions
- `PomodoroTimer/Styles/`: Kingston red, black, and white design tokens and shared control styling
- `PomodoroTimer.Desktop/Program.cs`: Windows/macOS desktop bootstrap
- `PomodoroTimer.Android/MainActivity.cs`: Android entry point
- `PomodoroTimer.iOS/AppDelegate.cs`: iOS entry point

## Getting Started

### Prerequisites

- .NET 8 SDK for the shared app, desktop head, tests, and CI path
- .NET 10 SDK plus Android/iOS workloads and native toolchains when building the mobile heads

### Build the default desktop/test workspace

```bash
dotnet build PomodoroTimer.sln
```

### Run the desktop app on Windows or macOS

```bash
dotnet run --project PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj
```

### Open the full cross-platform workspace

Use `PomodoroTimer.CrossPlatform.slnx` in an IDE when you need the desktop, Android, and iOS heads visible together.

### Build mobile heads explicitly

```bash
dotnet build PomodoroTimer.Android/PomodoroTimer.Android.csproj
dotnet build PomodoroTimer.iOS/PomodoroTimer.iOS.csproj
```

### Test

```bash
dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --no-build
```

## Current App Behavior

- The app starts on the timer page with a 25-minute work duration and 5-minute break duration
- Users can enter a focus topic before starting work
- Completing a full work session stores a completed focus session and increments today's pomodoro count
- Completing a break does not increment today's pomodoro count
- Resetting an incomplete work session does not create a completed session
- Settings and focus sessions persist under the local app-data directory as JSON
- The default language is Simplified Chinese, with Traditional Chinese and English available in-app

## Chinese Version

For a Chinese project introduction and setup guide, see [README.zh-CN.md](README.zh-CN.md).
