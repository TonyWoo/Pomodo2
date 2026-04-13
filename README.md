# PomodoroTimer

[中文说明 / Chinese documentation](README.zh-CN.md)

PomodoroTimer is a Pomodoro timer built with Avalonia. The repository now shares one app layer across Windows and macOS desktop, plus dedicated iOS and Android heads that host the same timer UI, localization, and state logic.

## Overview

- Focus session: 25 minutes
- Short break: 5 minutes
- Controls for start/pause, reset, and skipping to the next phase
- Progress tracking for the active phase and completed focus sessions
- One shared Avalonia view-model/model/localization stack across desktop and mobile heads

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
|   |-- Views/MainView.axaml
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
- `PomodoroTimer/Views/MainView.axaml`: shared timer shell used by desktop and mobile heads
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

- The app starts in a 25-minute focus session
- Completing a focus session switches to a 5-minute short break
- Completing a break switches back to focus mode
- Manually skipping a phase does not increment the completed-focus counter
- The current UI copy is written in Chinese by default, with additional language options available in-app

## Chinese Version

For a Chinese project introduction and setup guide, see [README.zh-CN.md](README.zh-CN.md).
