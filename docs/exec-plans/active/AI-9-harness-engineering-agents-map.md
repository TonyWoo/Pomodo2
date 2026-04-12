# AI-9 Harness Engineering Agents Map

## Scope

Add a root `AGENTS.md` map for this repository, add the supporting docs it
links to, and introduce a repo-local guardrail that mechanically validates the
required documentation structure without modifying the existing GitHub Actions
workflow.

## Assumptions

- The enforcement mechanism should stay native to the existing `.NET 8` +
  `xUnit` workflow rather than adding a new external service or package.
- `AGENTS.md` should remain concise and delegate deeper explanation to
  `docs/`.
- The existing `.github/workflows/dotnet-desktop.yml` file remains unchanged
  for this ticket.

## Steps

- [x] Reset the prior rework attempt by removing the stale Linear workpad and
  branching fresh from `origin/main`.
- [x] Capture the reproduction signal for the missing doc map on the fresh
  branch.
- [x] Run the `pull` workflow and record the clean sync result.
- [x] Add the root `AGENTS.md` file and supporting docs under `docs/`.
- [x] Add repository guardrail tests that validate docs structure and
  cross-references without depending on workflow edits.
- [ ] Run local validation, publish the branch, and attach the fresh PR.

## Validation

- [x] `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --filter FullyQualifiedName~RepositoryDocumentationGuardrailsTests`
- [x] `dotnet build PomodoroTimer.sln`
- [x] `dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj`

## Progress Log

- 2026-04-12: Removed the stale Linear workpad and restarted from `origin/main`
  on `tonywoo815/ai-9-harness-engineering-agents-doc-guardrails`.
- 2026-04-12: Reproduced the gap on the fresh branch; `rg --files -g
  "AGENTS.md" -g "docs/**" -g "*.md"` returned only `README.md`,
  `README.zh-CN.md`, and `WORKFLOW.md`.
- 2026-04-12: Recorded clean `pull` evidence against `origin/main`; the new
  local branch has no remote counterpart yet, and the merge returned `Already
  up to date.`.
- 2026-04-12: Added `AGENTS.md`, `docs/ARCHITECTURE.md`, the `docs/exec-plans/`
  structure, and a live `tech-debt-tracker.md` without changing
  `.github/workflows/dotnet-desktop.yml`.
- 2026-04-12: Added `RepositoryDocumentationGuardrailsTests` so the repo
  mechanically checks the required harness-doc files and cross-references.
- 2026-04-12: Local validation passed for the repo guardrails, solution build,
  and the full test suite.
