# Task: audit the base repo and remove all original-author identifiers

This is the first task, before any feature work. Goal: confirm the codebase is
functionally what the documentation claims, and strip every trace of the
original author/project identity so CloudWay doesn't ship someone else's name.
**Do not change any logic while doing this** — this is a search-and-report,
then a narrowly-scoped find-and-replace, nothing else.

## Step 1 — Report only (no edits yet)

Search the entire repo (code, config, comments, `.sln`/`.csproj` files, README,
LICENSE, any docs folders) for and list every occurrence of:

- The original author's name or initials, in any casing or spacing.
- The GitHub username the repo was published under.
- The project's original name ("Techspire", "CloudWay_LMS", "Techspire LMS")
  wherever it appears as literal text — page `<title>` tags, master page
  branding, `Web.sitemap`, favicon references, footer text, `AssemblyInfo.cs`
  (`AssemblyCompany`, `AssemblyProduct`, `AssemblyCopyright`, `AssemblyTitle`),
  `.sln` file comments, README.md, LICENSE.txt copyright line.
- Any student ID, email address, or personal identifier anywhere in the tree.
- Any commit-message or code-comment references to the original author.

Present this as a table: file path, line number, what was found.

## Step 2 — Confirm before touching anything

Show me the Step 1 report. I will confirm what to replace with before you
change anything — do not decide replacement text yourself for names/identity,
only ask.

## Step 3 — Apply agreed replacements

Once confirmed:
- Replace the project name everywhere with "CloudWay" (namespaces can stay as
  they are internally if renaming them is high-risk — flag this rather than
  silently renaming a namespace across the whole solution).
- Replace `AssemblyCompany`/`AssemblyProduct`/`AssemblyCopyright`/`AssemblyTitle`
  with placeholder values (e.g. your own group name), not the original author's.
- Replace README.md and LICENSE.txt content with your own (see note below).
- Remove any personal name/ID/email found in Step 1.
- Do **not** touch the actual business logic, SQL, or page-behind code beyond
  literal text/string replacements for the above.

## Note on licensing

The base repo carries an MIT license. Keep license compliance in mind: MIT
requires the original copyright notice to be retained in copies of the
license text itself (not scattered through the app UI). Practically: keep a
`THIRD_PARTY_NOTICE.md` in the repo root crediting the original template and
its license, remove the original name from anywhere it appears as live
application content (UI, page titles, branding), and cite the base repo in
the assignment report's references section. Ask me before deciding exact
wording here — this affects academic integrity framing and should be
reviewed, not auto-generated.

## What "done" looks like

- Zero hits for the original author's name/ID/username anywhere except (if we
  decide to keep it) a single `THIRD_PARTY_NOTICE.md` crediting the template.
- Zero hits for "Techspire" as literal application-facing text.
- Solution still builds and every existing page still loads exactly as before —
  this step changes text and metadata only, never behavior.
