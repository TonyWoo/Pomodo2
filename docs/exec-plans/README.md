# Execution Plans

Execution plans are the durable task log for non-trivial work. Use them when a
change needs more than a couple of obvious edits or when another agent may need
to resume the task later.

## Directory Layout

- [active/README.md](active/README.md): in-flight plans
- [completed/README.md](completed/README.md): finished plans retained for
  history and examples
- [tech-debt-tracker.md](tech-debt-tracker.md): small, ongoing debt items that
  should not be lost between tasks

## Plan Rules

- Use one markdown file per task under `active/`.
- Name files with the ticket identifier first, for example:
  `AI-9-harness-engineering-guide.md`
- Record scope, assumptions, progress notes, validation, and final disposition.
- Move the file to `completed/` when the work is merged or otherwise finished.
- Keep links repo-relative so another agent can follow the thread without
  external context.

## Lightweight Template

```md
# <ticket> <short title>

## Scope

## Assumptions

## Steps

- [ ] Step 1
- [ ] Step 2

## Validation

- [ ] `command`

## Progress Log

- YYYY-MM-DD: note
```
