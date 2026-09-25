# Assignment requirements — the actual grading criteria

Source: CT050-3-2-WAPP Web Applications group assignment brief. Nothing gets
built without a line here justifying it; nothing here gets skipped.

## Website requirements (the working system — this is what code targets)

| Requirement | Satisfied by | Status |
|---|---|---|
| Interlinked webpages | Shared master pages + nav/breadcrumb across all areas | Inherited from base repo |
| Proper use of HTML5 elements | Semantic markup in new CloudWay UI (nav, article, section, footer, etc.) | To build in UI phase |
| CSS: external | `Content/site.css`, rebuilt with new design | To build |
| CSS: internal | At least one page with a `<style>` block for page-specific rules | To build — explicit rubric item, don't skip |
| CSS: inline | At least one deliberate `style="..."` usage | To build — explicit rubric item, don't skip |
| Good quality content | Real AWS lesson/quiz content, not lorem ipsum | To seed |
| DB connectivity: Insert | Admin "Add" pages for Courses/Lessons/Quizzes/etc. | Inherited pattern |
| DB connectivity: Display | Catalogue, course details, admin grids | Inherited pattern |
| DB connectivity: Update | Admin "Edit" pages | Inherited pattern |
| DB connectivity: Delete | Admin "Delete" actions | Inherited pattern |
| Registration page (new members) | `Account/Register.aspx` | Inherited |
| Registered member module | Enrol, learn, quiz, progress, profile (Student role) | Inherited |
| Administrator module | Full CRUD over all entities, protected centrally | Inherited |
| Form validation | Client-side validators + server-side BLL checks on every form | Inherited pattern, extend to any new form |
| Navigation support | Consistent nav + breadcrumbs on every page | To rebuild visually, keep functionally |
| File organization / naming convention | Follow `05_REPO_STRUCTURE.md` exactly | Ongoing discipline |

## Documentation requirements (separate deliverable — not built into the website)

Do not build these as app features. They are Word/report deliverables,
produced once the system is stable, describing what was actually built:

- Proposal report (already submitted): title, objectives, mission statement,
  audience modelling, scope.
- Final report: cover page, table of contents, introduction/project plan,
  requirement specification (audience modelling, use cases, flowcharts, major
  functions), design and modelling (ERD, wireframes, navigation structure),
  implementation writeup (CSS approach, form validation approach, key SQL/DAL
  patterns — explained, not the full source dumped in), user guidance
  (screenshots + descriptions), conclusion, references, appendix.
- The Week 7 proposal document states ASP.NET Core MVC + Entity Framework
  Core + Bootstrap. **This is factually wrong for the system actually being
  built** (ASP.NET Web Forms + ADO.NET, per `02_ARCHITECTURE.md`). The
  assignment brief itself does not mandate any specific stack, so this is a
  documentation-consistency issue, not a technical one. Fix: the final
  report's "System Architecture and Technical Stack" section must describe
  Web Forms + ADO.NET as actually implemented — do not carry the proposal's
  wording forward unchanged. Flag this to the human when documentation work
  starts; do not silently leave it inconsistent.
- Full source code must **not** be included in the documentation — describe
  and reference it, don't paste it wholesale.
- Report formatting: Times New Roman 12pt (except headings), 1.5 line
  spacing, justified, numbered, APU cover page format. This is a
  Word-document concern, not a coding-agent concern — flag it back to the
  human when documentation work starts.

## Login / auth UX enhancements (confirmed scope, keep small)

Not required by the brief, but cheap and worth doing since login is one of
the first things a marker interacts with. Each one is a small, bounded
addition on top of the existing `AuthBLL`/`UserDAL` pattern — none of them
introduce a new layer, a new library, or a new page beyond what's listed.

1. **Role-aware post-login redirect + nav.** After login, send Admins to the
   admin dashboard and Students to "My Learning" instead of a generic
   homepage. Show the logged-in user's name and role in the shared nav
   (`Site.master`/`Admin.master`). Implementation: a redirect branch in the
   existing login page-behind based on `AuthBLL.IsAdmin`, plus one nav
   partial reading `AuthBLL.CurrentUserId`/role. No new BLL method needed.

2. **Visible lockout/attempt feedback.** The lockout logic in `AuthBLL.Login`
   already exists (5 attempts, 15-minute lockout) — just surface the
   remaining-attempts count in the error message instead of a generic
   "invalid email or password" on every failed try before lockout.
   Implementation: change the message string built in the existing method;
   no schema change, no new method.

3. **"Remember me" checkbox on login.** A persistent cookie holding a random
   token (never the password) so a session survives a browser restart.
   Implementation: one checkbox on `Login.aspx`; on success, generate a
   random token, store it + an expiry in the new `RememberToken`/
   `RememberTokenExpiry` columns on `Users` (see `03_DATA_MODEL.md`), and set
   a matching cookie; on future visits, `Global.asax` or the master page
   checks the cookie against the stored token before falling back to normal
   session login. Compare tokens with a constant-time or simple equality
   check the same way passwords are compared — never trust the cookie alone
   without checking it against the stored, still-valid token and expiry.

Explicitly **not** in scope: a "forgot password" flow. The base repo lists
this as a deliberate limitation (no email sending configured), and a fake
security-question-based reset adds real attack surface for very little
grading benefit. Keep it as a documented known limitation in the final report.

## Explicit non-goals (things the brief does not ask for — don't add scope)

- No requirement for a specific number of pages beyond what naturally covers
  the modules above.
- No requirement for mobile apps, payment systems, live video, or third-party
  integrations — the CloudWay proposal already scoped these out; don't
  reintroduce them as "nice to haves."
- No requirement for automated tests, CI/CD, containerization, or cloud
  deployment. Running locally in Visual Studio against local SQL Server is
  sufficient and expected.
