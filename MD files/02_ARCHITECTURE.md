# Architecture — strict, non-negotiable

## Stack (pinned — do not add or swap frameworks)

- C#, ASP.NET Web Forms, .NET Framework 4.8, Web Application Project (not
  "Website" project).
- Microsoft SQL Server (SQL Server Express for development).
- ADO.NET via a hand-written Data Access Layer. **No Entity Framework, no
  Dapper, no ORM of any kind.** The base repo does not use one and adding one
  now is a rewrite in disguise.
- Plain HTML5, CSS3, ASP.NET server controls. No SPA framework (no React,
  Angular, Vue) inside the Web Forms pages.
- Visual Studio 2019/2022.

Do not introduce a new package, framework, or library without asking first,
even a small one. Every added dependency is something the whole team has to
understand and something that can break the build on someone else's machine.

## The four layers, and the one rule that matters most

Request flow is always: **Pages (.aspx) → BLL → DAL → Database**, and never any
other direction or shortcut.

- **Models** — plain C# classes describing a "thing" (`Course`, `User`, `Quiz`).
  Properties only. No logic, no SQL.
- **Data_Access_Layer (DAL)** — the only layer allowed to contain `SqlCommand`
  or any SQL text. One class per table. Every query is parameterized — no
  string concatenation into SQL, ever, no exceptions.
- **BLL (Business Logic Layer)** — validation and business rules ("a course
  must have a title", "you can't enrol twice"). Throws `ValidationException`
  on a failed rule. This is the only layer allowed to call the DAL.
- **Pages (.aspx / .aspx.cs)** — handles the click, calls a BLL method, shows
  the result. Never calls a DAL class directly. Never builds SQL. Never trusts
  a query string or hidden field as identity — identity always comes from
  session (see Security below).

**Test for every change you make:** if you're about to write a `SqlCommand` or
`SELECT`/`INSERT`/`UPDATE`/`DELETE` anywhere outside `Data_Access_Layer/`, stop
— that's a violation. If you're about to call a DAL method from a `.aspx.cs`
file directly, stop — go through a BLL method instead.

## Security patterns to keep exactly as-is

- Passwords: salted hash only (never store or compare plain text). Reuse the
  existing hashing helper — do not write a new one.
- Sessions: authentication state lives in one place only (the existing
  `AuthBLL`-style class). Pages ask it "am I logged in / am I admin", they
  never touch `Session` directly.
- Admin authorization: enforced once, in the shared admin master page, not
  copy-pasted into every admin page's `Page_Load`.
- Identity for any action ("who is doing this") always comes from session,
  never from a query string, hidden field, or posted button argument. Only
  non-sensitive IDs (like a public course ID) may come from the URL.
- Every new form gets both client-side validators (instant feedback) and a
  server-side check in the corresponding BLL method (the actual boundary).
  Client-side alone is never sufficient.
- File uploads: extension allowlist, size cap, and a randomly generated
  filename — never trust or reuse the uploaded file's own name.

## What "keeping it simple" means architecturally

- No dependency injection container. No repository-pattern-on-top-of-DAL
  (the DAL already is the repository layer). No CQRS, no mediator pattern, no
  generic base-class gymnastics "for future flexibility."
- If a page needs data from two tables, call two BLL methods from the page, or
  add one purpose-built BLL method — don't build a generic query builder.
- New features slot into the existing four layers. If a feature seems to need
  a fifth layer or a different pattern, that's a signal to simplify the
  feature, not to add architecture.
