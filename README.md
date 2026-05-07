# PomodoroTimer

[中文说明 / Chinese documentation](README.zh-CN.md)

PomodoroTimer is a Pomodoro timer built with Avalonia. The repository now shares one app layer across Windows and macOS desktop, plus dedicated iOS and Android heads that host the same timer UI, localization, and state logic.

## Overview

PomodoroTimer is a full-featured productivity application that combines the Pomodoro Technique with task management and statistics tracking. The app shares one unified codebase across Windows, macOS desktop, iOS, and Android platforms.

## Features

- **Pomodoro Timer**
  - Customizable work duration (default 25 minutes)
  - Customizable break duration (default 5 minutes)
  - Start/pause, reset, and skip controls
  - Visual progress indicator with circular timer display
  
- **Task Management**
  - Create and manage daily tasks
  - Track completed pomodoros per task
  - Mark tasks as complete or delete them
  - Tasks persist across app sessions
  
- **Statistics & History**
  - View daily statistics and focus time
  - Browse session history with timestamps
  - Track productivity trends over time
  
- **Multi-Language Support**
  - Built-in localization system
  - Switch between languages in-app
  - Persistent language preference
  
- **Multi-Page Navigation**
  - Timer: Main pomodoro interface with task list
  - Stats: Daily statistics and session history
  - Settings: Customize durations and language
  - About: App version and information
  
- **Cross-Platform**
  - Responsive layout for desktop and mobile
  - Shared UI and business logic across all platforms
  - Platform-specific optimizations

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
|   |-- Views/
|   |   |-- MainView.axaml
|   |   |-- MainWindow.axaml
|   |   |-- TimerView.axaml
|   |   |-- StatsView.axaml
|   |   |-- SettingsView.axaml
|   |   `-- AboutView.axaml
|   |-- ViewModels/
|   |   |-- MainWindowViewModel.cs
|   |   |-- TimerViewModel.cs
|   |   |-- StatsViewModel.cs
|   |   |-- SettingsViewModel.cs
|   |   |-- AboutViewModel.cs
|   |   |-- TaskListItemViewModel.cs
|   |   `-- SessionListItemViewModel.cs
|   |-- Models/
|   |   |-- PomodoroTimerState.cs
|   |   |-- TodayTask.cs
|   |   |-- FocusSession.cs
|   |   |-- DailyStats.cs
|   |   `-- AppSettings.cs
|   |-- Services/
|   |   |-- TimerService.cs
|   |   |-- JsonTaskStore.cs
|   |   |-- JsonSessionStore.cs
|   |   |-- JsonSettingsStore.cs
|   |   `-- ITaskStore.cs / ISessionStore.cs / ISettingsStore.cs
|   `-- Localization/
|       |-- AppLocalizer.cs
|       |-- LocalizedText.cs
|       `-- LanguageOption.cs
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
- `PomodoroTimer/Views/MainView.axaml`: shared timer shell with multi-page navigation
- `PomodoroTimer/ViewModels/TimerViewModel.cs`: timer logic, task coordination, and UI state
- `PomodoroTimer/ViewModels/StatsViewModel.cs`: statistics calculation and session history
- `PomodoroTimer/Models/PomodoroTimerState.cs`: timer state machine and phase transitions
- `PomodoroTimer/Models/TodayTask.cs`: task model with pomodoro counting
- `PomodoroTimer/Services/TimerService.cs`: core timer service with settings support
- `PomodoroTimer/Services/JsonTaskStore.cs`: task persistence layer
- `PomodoroTimer/Localization/AppLocalizer.cs`: multi-language support and string management
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

### Timer and Task Management

- The app starts on the Timer page with a 25-minute focus session
- Users can create daily tasks before or during a pomodoro session
- Starting a timer without an active task creates an "Untitled" task automatically
- Completing a focus session:
  - Increments the pomodoro count for the active task
  - Saves the session to history
  - Switches to a 5-minute short break
- Completing a break switches back to focus mode
- Manually skipping a phase does not increment the completed-focus counter
- Tasks can be marked as complete or deleted
- All tasks and sessions persist across app restarts

### Navigation and Settings

- Navigate between Timer, Stats, Settings, and About pages using the sidebar (desktop) or bottom navigation (mobile)
- Stats page shows daily statistics and session history
- Settings page allows customization of work/break durations and language selection
- The default UI language is Chinese (Simplified), with additional language options available in-app
- All settings persist across app sessions

## Chinese Version

For a Chinese project introduction and setup guide, see [README.zh-CN.md](README.zh-CN.md).
