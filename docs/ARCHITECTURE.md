# Architecture

## Runtime Shape

The solution has one desktop application project and one test project:

- `PomodoroTimer/`: Avalonia desktop app targeting `net8.0`
- `PomodoroTimer.Tests/`: xUnit regression coverage for timer, localization,
  view-model behavior, and repository guardrails

## Layer Map

1. Desktop bootstrap
   - `PomodoroTimer/Program.cs` starts the Avalonia desktop lifetime.
   - `PomodoroTimer/App.axaml` and `App.axaml.cs` own app-wide resources and
     startup wiring.
2. Views
   - `PomodoroTimer/Views/MainWindow.axaml` defines the visible desktop layout.
   - Code-behind should stay thin; behavior belongs elsewhere unless Avalonia
     requires view-specific wiring.
3. Presentation logic
   - `PomodoroTimer/ViewModels/MainWindowViewModel.cs` owns commands,
     calculated labels, progress state, and localization bindings.
   - `DispatcherTimer`-driven UI ticking lives here, not in the domain model.
4. Domain model
   - `PomodoroTimer/Models/PomodoroTimerState.cs` owns focus/break timing rules,
     session completion, and phase switching.
   - Keep timer rules deterministic and easy to exercise from xUnit.
5. Localization
   - `PomodoroTimer/Localization/` owns language selection, translated strings,
     and persistence of the language preference.

## Change Guidance

- Change `Models/` first when timer rules or state transitions change.
- Change `ViewModels/` when commands, derived text, or progress calculations
  change.
- Change `Localization/` when introducing or editing user-facing copy.
- Change `Views/` when layout or bindings change.
- Add or update tests in `PomodoroTimer.Tests/` whenever behavior or localized
  output changes.

## Validation Path

- Build with `dotnet build PomodoroTimer.sln`
- Run tests with `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`
- Run the repo guardrails with
  `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --filter FullyQualifiedName~RepositoryDocumentationGuardrailsTests`
- Track any additional missing guardrails in
  [exec-plans/tech-debt-tracker.md](exec-plans/tech-debt-tracker.md)
