# AI-2026-05-01 Main Shell Task Management And Icon Refresh

## Scope

- Replace the current tomato mark used in the shell and timer views with a generated bitmap asset.
- Fix sidebar and compact bottom navigation alignment so icon and label stay on one line and the compact navigation is centered with a fuller background container.
- Add real task input and task management for today's task list instead of showing only completed focus sessions.

## Assumptions

- Task management can be persisted locally alongside existing settings and session files.
- The "today tasks" panel should show tasks created for the current day immediately, before a pomodoro finishes.
- Completing a pomodoro should attach the finished session to the active task when one exists.

## Steps

- [x] Add a durable task model, task store, and timer/task coordination in the view model layer.
- [x] Update desktop and compact timer UI to support adding, listing, toggling, and removing today's tasks.
- [x] Refresh shell navigation layout and swap tomato visuals to a generated bitmap asset.
- [x] Add or update tests for task persistence and timer/task interactions.
- [x] Validate with build and test commands.

## Validation

- [x] `dotnet build PomodoroTimer.sln --no-restore`
- [x] `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --no-build`

## Progress Log

- 2026-05-01: Created plan and started inspecting timer, shell, store, and navigation ownership files.
- 2026-05-01: Added a persisted today-task model/store, rewired timer task handling, refreshed shell/timer layout, generated and imported a new tomato bitmap asset, and passed build plus tests.
