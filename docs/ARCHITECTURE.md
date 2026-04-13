# Architecture

## Runtime Shape

The repository now has one shared Avalonia app project, three platform heads,
and one test project:

- `PomodoroTimer/`: shared app layer targeting `net8.0`
- `PomodoroTimer.Desktop/`: Windows/macOS desktop head targeting `net8.0`
- `PomodoroTimer.Android/`: Android head targeting `net10.0-android`
- `PomodoroTimer.iOS/`: iOS head targeting `net10.0-ios`
- `PomodoroTimer.Tests/`: xUnit regression coverage for timer, localization,
  view-model behavior, and repository guardrails

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
   - `PomodoroTimer/App.axaml` and `App.axaml.cs` own app-wide resources and
     choose the correct Avalonia lifetime entry for desktop or mobile.
3. Views
   - `PomodoroTimer/Views/MainView.axaml` defines the shared timer shell.
   - `PomodoroTimer/Views/MainWindow.axaml` stays as the desktop-specific host
     window around the shared view.
4. Presentation logic
   - `PomodoroTimer/ViewModels/MainWindowViewModel.cs` owns commands,
     calculated labels, progress state, and localization bindings.
   - `DispatcherTimer`-driven UI ticking lives here, not in the domain model.
5. Domain model
   - `PomodoroTimer/Models/PomodoroTimerState.cs` owns focus/break timing
     rules, session completion, and phase switching.
   - Keep timer rules deterministic and easy to exercise from xUnit.
6. Localization
   - `PomodoroTimer/Localization/` owns language selection, translated strings,
     and persistence of the language preference.

## Change Guidance

- Change `Models/` first when timer rules or state transitions change.
- Change `ViewModels/` when commands, derived text, or progress calculations
  change.
- Change `Localization/` when introducing or editing user-facing copy.
- Change `Views/` when layout or bindings change.
- Change `PomodoroTimer.Desktop/`, `PomodoroTimer.Android/`, or
  `PomodoroTimer.iOS/` when platform bootstrapping or packaging changes.
- Add or update tests in `PomodoroTimer.Tests/` whenever behavior or localized
  output changes.

## Validation Path

- Build the default workspace with `dotnet build PomodoroTimer.sln`
- Run tests with `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`
- Run the repo guardrails with
  `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --filter FullyQualifiedName~RepositoryDocumentationGuardrailsTests`
- Build mobile heads explicitly with `dotnet build PomodoroTimer.Android/PomodoroTimer.Android.csproj` and `dotnet build PomodoroTimer.iOS/PomodoroTimer.iOS.csproj` when the `.NET 10` mobile SDK workloads and host toolchains are available
- Track any additional missing guardrails in
  [exec-plans/tech-debt-tracker.md](exec-plans/tech-debt-tracker.md)
