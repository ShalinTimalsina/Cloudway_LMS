# Data model — the only tables and columns that exist

This is the single source of truth for the database. **Do not add, rename, or
remove a table or column without updating this file first and getting
confirmation.** If code references a column not listed here, that's a bug to
fix, not a reason to add the column silently.

Fifteen tables, unchanged in structure from the base repo. Only the seed
*content* changes to be AWS-specific — see the mapping at the bottom.

## Tables

**Roles** — `RoleID` (PK). The two roles: Admin, Student.

**Users** — `UserID` (PK), `RoleID` (FK), FullName, Email (unique),
PasswordHash, PasswordSalt, IsActive, FailedLoginAttempts, LockoutEndUtc,
CreatedAt, `RememberToken` (nullable), `RememberTokenExpiry` (nullable,
datetime). The last two support "remember me" — see the auth UX section in
`04_ASSIGNMENT_REQUIREMENTS.md`. Nothing else added to this table.

**Categories** — `CategoryID` (PK), Name, Description.
AWS domains: Compute, Storage, Networking, Security & IAM, Databases.

**Courses** — `CourseID` (PK), `CategoryID` (FK), `CreatedBy` (FK → Users),
Title, Description, ThumbnailPath, IsPublished, CreatedAt.

**Lessons** — `LessonID` (PK), `CourseID` (FK), Title, Content, VideoUrl,
OrderIndex. See "Lesson content decision" below for how `Content` and
`VideoUrl` are actually used in this phase.

**Resources** — `ResourceID` (PK), `LessonID` (FK), FileName, FilePath,
UploadedAt. Downloadable attachments per lesson. Table stays in the schema;
not populated or built out in this phase — see below.

**Tags** — `TagID` (PK), Name.
AWS certification tracks: Cloud Practitioner, Solutions Architect Associate,
Developer Associate.

**CourseTags** — `CourseID` (FK), `TagID` (FK). Many-to-many junction.

**Enrollments** — `EnrollmentID` (PK), `UserID` (FK), `CourseID` (FK),
EnrolledAt, ProgressPercent. Unique on (UserID, CourseID).

**LessonProgress** — `EnrollmentID` (FK), `LessonID` (FK), CompletedAt.
Composite key (EnrollmentID, LessonID).

**Quizzes** — `QuizID` (PK), `CourseID` (FK), Title, PassingScore.

**Questions** — `QuestionID` (PK), `QuizID` (FK), QuestionText.

**QuestionOptions** — `OptionID` (PK), `QuestionID` (FK), OptionText,
IsCorrect.

**QuizAttempts** — `AttemptID` (PK), `UserID` (FK), `QuizID` (FK), Score,
AttemptedAt. Permanent record — never deleted, this is what powers the
student's score-history dashboard.

**Feedback** — `FeedbackID` (PK), `UserID` (FK, nullable — guests can submit
with no account), Name, Email, Message, SubmittedAt.

## Cardinalities (must match exactly)

- Roles 1—N Users
- Categories 1—N Courses
- Courses 1—N Lessons, 1—N Quizzes, 1—N Enrollments
- Lessons 1—N Resources, 1—N LessonProgress
- Users 1—N Enrollments, 1—N QuizAttempts, 1—N Feedback (nullable)
- Enrollments 1—N LessonProgress
- Courses N—M Tags (via CourseTags)
- Quizzes 1—N Questions, 1—N QuizAttempts
- Questions 1—N QuestionOptions

Deleting a Category cascades to its Courses, Lessons, Quizzes, Questions and
QuestionOptions — but never to Enrollments or QuizAttempts. A learner's
history must survive even if the underlying course content is later removed.

## Lesson content decision (text-only phase)

For this phase, lessons are text-only. This does not change the schema —
`VideoUrl` and `Resources` stay exactly as defined above — it only decides how
these columns/tables are populated and rendered right now:

- **`Content`** stores a small, deliberately limited set of basic HTML, not
  plain text and not a full rich-text/WYSIWYG payload: `<p>`, `<strong>`,
  `<em>`, `<ul>`/`<ol>`/`<li>`, and `<h3>`/`<h4>` for in-lesson subheadings.
  Nothing else — no inline styles, no scripts, no arbitrary tags. Rendered on
  `LessonDetails.aspx` via a `Literal` control (`Mode = Literal.HtmlEncode` is
  OFF for this control only, since we want the HTML to render — every other
  user-facing text field on the site stays HTML-encoded as normal).
- Because `Content` renders as raw HTML, it is never populated from
  unsanitized user/guest input — only from admin-entered or seeded course
  content. If an Admin "edit lesson" form is built later, treat this as a
  stored-content field for a trusted admin role, not a public form field; if
  that assumption ever changes, this file must be revisited before shipping
  it, since raw HTML rendering plus untrusted input is an XSS risk.
- **`VideoUrl`** stays `NULL` for every seeded lesson this phase. The lesson
  page checks for `NULL`/empty and simply omits the video section rather than
  rendering a broken player — do not build embed/player logic yet.
- **`Resources`** stays empty this phase — no upload UI, no file-serving code
  yet. The table exists for a later phase, not now.

Revisit this section (and only this section — not the schema above) if video
or downloadable resources are added in a later phase.

## AWS content mapping (for seeding, not schema)

| Original generic example | CloudWay/AWS content |
|---|---|
| Category: "Web Development" | Category: "Compute" |
| Course: "Intro to HTML" | Course: "EC2 Fundamentals" |
| Category: "Databases" | Category: "Databases" (kept — RDS, DynamoDB courses) |
| Tag: generic skill tag | Tag: "Solutions Architect Associate" |

Keep the content set small: 4–5 categories, 2–3 courses each, 3–5 lessons per
course, one quiz per course with 4–6 questions. This is enough to demonstrate
every CRUD path and every user flow without needing to write a large amount
of content by hand.
