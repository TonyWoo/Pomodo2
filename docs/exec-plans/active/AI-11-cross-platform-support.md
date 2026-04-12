# AI-11 Cross-Platform Support

## Scope

Shift the repository from a desktop-only Avalonia app shape to a shared app
layer with explicit Windows/macOS desktop, Android, and iOS entry points,
while keeping the default local validation path stable.

## Assumptions

- Windows and macOS can share the same Avalonia desktop head.
- The default `PomodoroTimer.sln` should stay buildable in environments that do
  not have Android/iOS workloads installed.
- Mobile build validation in this workspace may be limited by platform-specific
  workloads and host OS constraints, so repo structure and guardrails must carry
  part of the support signal.

## Steps

- [x] Capture the desktop-only baseline and required pull evidence.
- [x] Convert `PomodoroTimer/` into the shared Avalonia app layer and add a
  dedicated desktop head.
- [x] Add Android and iOS platform heads plus a full cross-platform solution
  file.
- [x] Update documentation and repository guardrails so the new platform layout
  is explicit and enforced.
- [ ] Run validation, clean temporary artifacts, publish the branch, and attach
  the PR.

## Validation

- [ ] `dotnet build PomodoroTimer.sln`
- [ ] `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`
- [ ] `dotnet build PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj`

## Progress Log

- 2026-04-12: Reproduced the gap: the solution only contained `PomodoroTimer`
  and `PomodoroTimer.Tests`, and no Android/iOS/mobile lifetime types were
  present in the repository.
- 2026-04-12: Recorded a clean `pull` sync against `origin/main` before any
  code edits.
- 2026-04-12: Split the repo into a shared app layer plus Windows/macOS
  desktop, Android, and iOS heads, and added `PomodoroTimer.CrossPlatform.slnx`
  to expose the full workspace.
- 2026-04-12: Reworked README/architecture/AGENTS/guardrails/CI so the new
  platform support is documented and mechanically checked.
