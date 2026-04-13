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

## Status

Blocked on environment handoff requirements. The shared desktop/mobile repo
shape is implemented locally, but this host cannot validate or publish the
mobile heads end to end.

## Steps

- [x] Capture the desktop-only baseline and required pull evidence.
- [x] Convert `PomodoroTimer/` into the shared Avalonia app layer and add a
  dedicated desktop head.
- [x] Add Android and iOS platform heads plus a full cross-platform solution
  file.
- [x] Update documentation and repository guardrails so the new platform layout
  is explicit and enforced.
- [x] Revalidate the current branch for desktop/test/sync status.
- [ ] Complete blocked mobile/runtime/publish validation in a session with the
  required SDK/tooling.

## Validation

- [x] `dotnet build PomodoroTimer.sln`
- [x] `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`
- [x] `dotnet build PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj`
- [ ] `dotnet build PomodoroTimer.Android/PomodoroTimer.Android.csproj`
- [ ] `dotnet build PomodoroTimer.iOS/PomodoroTimer.iOS.csproj`
- [ ] app runtime/media handoff tooling (`launch-app`, `github-pr-media`)

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
- 2026-04-12: Re-ran `dotnet build PomodoroTimer.sln`,
  `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`, and
  `dotnet build PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj` serially;
  all passed on commit `87b2e1b`.
- 2026-04-12: Confirmed the branch already contains current `origin/main`
  (`2531373`) after `git fetch origin`.
- 2026-04-12: Inspected local `Avalonia.Android` and `Avalonia.iOS` NuGet
  assets; version `12.0.0` only ships `net10.0-android*` and `net10.0-ios*`
  assemblies, so `.NET SDK 8.0.302` cannot satisfy the mobile builds on this
  host.
- 2026-04-12: Blocked handoff remains: a session with a `.NET 10` SDK, the
  required runtime/media workflow tools, and publish-capable GitHub credentials
  is needed to finish mobile validation and PR publication.