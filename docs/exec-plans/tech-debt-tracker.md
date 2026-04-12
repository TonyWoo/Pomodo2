# Tech Debt Tracker

Track small, concrete gaps that reduce agent legibility or mechanical
enforcement. Add items here instead of letting them live only in comments or
memory.

| ID | Status | Area | Gap | Impact | Intended guardrail |
| --- | --- | --- | --- | --- | --- |
| TD-001 | Open | CI | `.github/workflows/dotnet-desktop.yml` is still the stock desktop template with placeholder environment variables and packaging steps unrelated to this Avalonia app. | Agents cannot rely on CI to enforce the documented `dotnet build` and `dotnet test` path yet. | Replace the placeholder workflow with a repo-specific workflow that restores, builds `PomodoroTimer.sln`, and runs `PomodoroTimer.Tests`. |
