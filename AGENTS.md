# AGENTS.md

## Purpose

This repository is a .NET 8 / Avalonia cross-platform Pomodoro timer. Treat the
repository as the source of truth for product behavior, architecture, and
execution history.

## Start Here

- Product and local setup: [README.md](README.md)
- Chinese product/setup overview: [README.zh-CN.md](README.zh-CN.md)
- Orchestration workflow: [WORKFLOW.md](WORKFLOW.md)
- Architecture map: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- Plans and debt tracking: [docs/exec-plans/README.md](docs/exec-plans/README.md)

## Repo Map

- `PomodoroTimer/App.axaml`: application-wide resources
- `PomodoroTimer/App.axaml.cs`: shared startup wiring for desktop and mobile
- `PomodoroTimer/Views/MainView.axaml`: shared timer shell for desktop/mobile
- `PomodoroTimer/Views/MainWindow.axaml`: desktop window host
- `PomodoroTimer.Desktop/Program.cs`: desktop bootstrap for Windows/macOS
- `PomodoroTimer.Android/MainActivity.cs`: Android head entry point
- `PomodoroTimer.iOS/AppDelegate.cs`: iOS head entry point
- `PomodoroTimer/ViewModels/MainWindowViewModel.cs`: commands, derived UI state,
  and localization bindings
- `PomodoroTimer/Models/PomodoroTimerState.cs`: bindable timer state snapshot
- `PomodoroTimer/Services/TimerService.cs`: timer state machine and phase
  transitions
- `PomodoroTimer/Localization/`: language options, localized strings, and
  preference persistence
- `PomodoroTimer.CrossPlatform.slnx`: optional full workspace with desktop,
  Android, and iOS heads
- `PomodoroTimer.Tests/RepositoryDocumentationGuardrailsTests.cs`: repo-level
  guardrails for agent docs and structural references
- `.github/workflows/dotnet-desktop.yml`: CI for `main` pushes and pull
  requests

## Operating Principles

1. Repository artifacts are the only durable knowledge for agents. If a
   decision matters, write it into code, docs, tests, or plans.
2. Keep `AGENTS.md` short. Put deep guidance in `docs/` and link to it from
   here.
3. Prefer mechanical enforcement over prose. Use tests, build commands, and CI
   checks when changing rules.
4. Treat plans as versioned artifacts in `docs/exec-plans/active/` and move
   them to `docs/exec-plans/completed/` when work lands.
5. Track small technical debt in `docs/exec-plans/tech-debt-tracker.md`
   instead of letting it accumulate.
6. When blocked, add missing context, tooling, or constraints to the
   repository before retrying the task.

## Change Workflow

1. Reproduce the current behavior or gap and capture the signal in the task log
   or plan.
2. Read the smallest source of truth that owns the area you are changing.
3. Prefer minimal edits in the owning layer:
   - UI layout or styling: `PomodoroTimer/Views/`
   - UI state or commands: `PomodoroTimer/ViewModels/`
   - Domain behavior: `PomodoroTimer/Models/`
   - Strings or language behavior: `PomodoroTimer/Localization/`
   - Platform bootstrap: `PomodoroTimer.Desktop/`, `PomodoroTimer.Android/`,
     `PomodoroTimer.iOS/`
4. Update tests when app behavior, derived state, or localization output
   changes.
5. Update docs when commands, structure, or operating rules change.
6. Before handoff, run:
   - `dotnet build PomodoroTimer.sln`
   - `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`
7. If a desired rule is not yet enforced, add a debt entry describing the
   missing check and the intended mechanical guardrail.

## Writing Plans

- Use one markdown file per non-trivial task under `docs/exec-plans/active/`.
- Include scope, assumptions, progress notes, validation, and final
  disposition.
- Keep plan status current so another agent can resume without external
  context.

## Documentation Hygiene

- Prefer repo-relative links and concrete file paths.
- Remove stale guidance in the same change that makes it obsolete.
- Do not duplicate full explanations here when a deeper doc already exists.
