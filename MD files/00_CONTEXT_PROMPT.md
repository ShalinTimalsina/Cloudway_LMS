# Context prompt — paste this first, before any other instruction

You are working inside a cloned GitHub repository (originally `Sampaadaa/LMS_WEB_APP`,
internally named `CloudWay_LMS`). This is a **base template only** — a working
ASP.NET Web Forms + SQL Server LMS written by someone else for a different purpose.
We are building **CloudWay**, an AWS-focused cloud learning platform, on top of its
architecture and database design, for a university group assignment
(CT050-3-2-WAPP — Web Applications).

## Before you write or change a single line of code

Read these files, in this order, in full:

1. `01_AUDIT_AND_SANITIZE.md` — do this task first and report back before anything else.
2. `02_ARCHITECTURE.md` — the layers and rules you must never violate.
3. `03_DATA_MODEL.md` — the only tables, columns and relationships that exist. Do not invent, rename, or drop any of them without asking first.
4. `04_ASSIGNMENT_REQUIREMENTS.md` — the actual grading requirements. Nothing gets built that isn't traceable to a line in this file, and nothing required in this file gets skipped.
5. `05_REPO_STRUCTURE.md` — the exact folder layout. Every new file goes into one of these folders; no new top-level folders without asking first.
6. `06_CODING_GUARDRAILS.md` — the anti-overengineering rules. This is a college assignment, not production software.

A `07_DESIGN.md` covering colours, typography and visual identity will be provided
separately before UI work starts — do not invent a visual design in the meantime.

## Standing rules for this entire project

- **Never guess a table name, column name, or file path.** If it isn't in
  `03_DATA_MODEL.md` or `05_REPO_STRUCTURE.md`, stop and ask rather than inventing
  one that "seems right." This is the single most important rule — hallucinated
  schema or paths break builds silently and waste hours.
- **Work in phases, one at a time, and stop for confirmation between phases.**
  Never do "everything in one pass."
- **Keep the build green.** After every phase, the project must still compile and
  the flows touched so far must still work, before moving on.
- **Prefer the smallest change that satisfies the requirement.** No new
  architectural layers, no new frameworks, no speculative abstraction "in case we
  need it later." See `06_CODING_GUARDRAILS.md`.
- **All SQL stays parameterized**, following the existing DAL pattern exactly —
  no string concatenation into SQL, ever, in old or new code.
- **This is our own project, not a reskin.** The database structure and layered
  architecture are reused deliberately (that's normal engineering practice and is
  disclosed in our report). The UI, branding, and content are being built from
  scratch and must contain no trace of the original author or project name — see
  the audit task.

## Your first response

Read all six files, then reply with:
1. Confirmation you've read them.
2. The audit findings from `01_AUDIT_AND_SANITIZE.md` (a list, not fixes yet).
3. Anything in the repo that conflicts with `03_DATA_MODEL.md` or
   `05_REPO_STRUCTURE.md`, so we resolve conflicts before writing new code.

Do not start building anything yet.
