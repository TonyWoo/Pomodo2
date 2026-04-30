# Architecture

## Runtime Shape

The repository has one shared Avalonia app project, three platform heads, and
one test project:

- `PomodoroTimer/`: shared app layer targeting `net8.0`
- `PomodoroTimer.Desktop/`: Windows/macOS desktop head targeting `net8.0`
- `PomodoroTimer.Android/`: Android head targeting `net10.0-android`
- `PomodoroTimer.iOS/`: iOS head targeting `net10.0-ios`
- `PomodoroTimer.Tests/`: xUnit regression coverage for timer, persistence,
  localization, view-model behavior, and repository guardrails

`PomodoroTimer.sln` remains the default local/CI validation surface for the
shared app, desktop head, and tests. `PomodoroTimer.CrossPlatform.slnx` adds
the mobile heads for IDE workflows with the required workloads installed.

## Layer Map

1. Platform bootstrap
   - `PomodoroTimer.Desktop/Program.cs` starts the classic desktop lifetime.
   - `PomodoroTimer.Android/MainActivity.cs` and `Application.cs` host the
     shared app inside Avalonia Android lifetimes.
   - `PomodoroTimer.iOS/AppDelegate.cs` and `Main.cs` host the shared app on
     iOS.
2. Shared app shell
   - `PomodoroTimer/App.axaml` loads the Fluent theme plus Kingston design
     tokens and control styles.
   - `PomodoroTimer/App.axaml.cs` creates JSON-backed settings/session stores,
     loads persisted settings, and routes desktop/mobile lifetimes into the
     same root view model.
3. Views
   - `PomodoroTimer/Views/MainView.axaml` defines the responsive shell and
     desktop/compact navigation.
   - `PomodoroTimer/Views/MainWindow.axaml` stays as the desktop-specific host
     window around the shared view.
   - `PomodoroTimer/Views/TimerView.axaml` owns the timer page layout:
     circular progress, topic entry, timer controls, today count, and records.
   - `PomodoroTimer/Views/StatsView.axaml` owns today's stats and record list.
   - `PomodoroTimer/Views/SettingsView.axaml` owns duration presets, custom
     duration inputs, and language selection.
4. Presentation logic
   - `PomodoroTimer/ViewModels/MainWindowViewModel.cs` owns shell navigation,
     app title/tagline bindings, and child view-model composition.
   - `TimerViewModel`, `StatsViewModel`, and `SettingsViewModel` own page
     commands, derived labels, and localized UI state.
   - Dispatcher timers live in the timer view model as presentation refresh
     plumbing; countdown rules live in services.
5. Domain model
   - `PomodoroTimer/Models/AppSettings.cs`, `FocusSession.cs`, `DailyStats.cs`,
     `TimerMode.cs`, and `TimerStatus.cs` describe persisted settings,
     completed work sessions, daily summary data, and timer state.
   - `PomodoroTimer/Models/PomodoroTimerState.cs` is a bindable snapshot shape;
     timer behavior is exercised through `TimerService`.
6. Services
   - `TimerService` performs drift-resistant countdown calculations from the
     target end time and creates completed work sessions only when work reaches
     zero.
   - `JsonSettingsStore` and `JsonSessionStore` persist settings and focus
     sessions under a cross-platform app-data directory.
   - `AppDataPathProvider` owns the app-data directory decision.
7. Localization
   - `PomodoroTimer/Localization/` owns language options, translated strings,
     language-code parsing, and legacy language preference compatibility.

## Change Guidance

- Change `Models/` and `Services/TimerService.cs` first when timer rules or
  state transitions change.
- Change `Services/JsonSettingsStore.cs` or `JsonSessionStore.cs` when local
  persistence rules change.
- Change `ViewModels/` when commands, navigation, derived text, or page state
  change.
- Change `Localization/` when introducing or editing user-facing copy.
- Change `Styles/` when Kingston color, typography, or shared control tokens
  change.
- Change `Views/` when layout or bindings change.
- Change `PomodoroTimer.Desktop/`, `PomodoroTimer.Android/`, or
  `PomodoroTimer.iOS/` when platform bootstrapping or packaging changes.
- Add or update tests in `PomodoroTimer.Tests/` whenever behavior, persistence,
  localized output, or derived state changes.

## Validation Path

- Build the default workspace with `dotnet build PomodoroTimer.sln`
- Run tests with `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`
- Run the repo guardrails with
  `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --filter FullyQualifiedName~RepositoryDocumentationGuardrailsTests`
- Build mobile heads explicitly with `dotnet build PomodoroTimer.Android/PomodoroTimer.Android.csproj` and `dotnet build PomodoroTimer.iOS/PomodoroTimer.iOS.csproj` when the `.NET 10` mobile SDK workloads and host toolchains are available
- Track any additional missing guardrails in
  [exec-plans/tech-debt-tracker.md](exec-plans/tech-debt-tracker.md)
