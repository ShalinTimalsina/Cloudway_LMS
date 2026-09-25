# Coding guardrails — this is a college assignment, not production software

The goal is a working, gradeable system built and understood by the team, not
an impressive-looking but over-engineered one nobody on the team can explain
in a viva. When in doubt, choose the option with less code, fewer files, and
fewer new concepts.

## Hard rules

- **One feature at a time, one phase at a time.** Never bundle "while I was in
  there I also refactored X." If you notice something else worth fixing,
  mention it and ask — don't do it silently in the same change.
- **No new architectural layers or patterns.** No DI container, no generic
  repository/unit-of-work on top of the existing DAL, no CQRS, no mediator, no
  event bus, no microservices split. Four layers, as defined in
  `02_ARCHITECTURE.md`, is the whole architecture.
- **No new NuGet/npm packages** without asking first and stating what problem
  it solves that the existing stack can't.
- **No speculative abstraction.** Don't build a generic `Repository<T>`, a
  configurable rules engine, or a plugin system "in case we need it later."
  Write the specific code the specific feature needs.
- **No premature optimization.** This system will have a handful of users and
  a few hundred rows per table at most. Don't add caching layers, background
  jobs, or query optimization that isn't needed at this scale.
- **Small diffs.** A single change should touch the smallest number of files
  that correctly implements the one thing being asked for.
- **Comment sparingly and usefully.** Comment *why*, not *what* — the code
  should already say what it does. Don't add a comment above every line.
- **Keep the build green after every phase.** If a change breaks the build or
  an existing flow, that's fixed before moving to the next task, not left for
  later.

## Signs you're overengineering (stop and simplify if you notice these)

- You're writing an interface with only one implementation, ever.
- You're adding a configuration option nobody asked for and won't change.
- You're generalizing a one-off admin page into a "generic CRUD framework."
- You're touching more than 3–4 files for what was described as a small
  feature.
- You're introducing a new naming pattern that doesn't match
  `05_REPO_STRUCTURE.md`'s existing conventions.
- You're solving a problem the assignment doesn't actually have (see
  `04_ASSIGNMENT_REQUIREMENTS.md`'s non-goals).

## Definition of done for any single task

1. It satisfies a specific line in `04_ASSIGNMENT_REQUIREMENTS.md`, or is the
   one agreed original feature, or is explicitly approved by the human first.
2. It follows `02_ARCHITECTURE.md`'s layering exactly.
3. It uses only tables/columns from `03_DATA_MODEL.md` — nothing invented.
4. It lives in the correct folder per `05_REPO_STRUCTURE.md`.
5. The solution builds with zero errors, and the specific flow just changed
   has been walked through (build → run → click through it) before calling
   the task complete.
