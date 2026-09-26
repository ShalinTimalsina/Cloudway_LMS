# Repository structure — strict

Every file belongs in exactly one of these folders. No new top-level folder
without asking first — a new folder is an architecture decision, not a
convenience.

```
CloudWay/
|
|-- Models/                 Plain C# classes: one per "thing" in the system
|                            (Course, User, Quiz...). Properties only.
|
|-- Data_Access_Layer/       ALL SQL lives here, and only here. One class per
|                            table (CourseDAL.cs, UserDAL.cs...), plus
|                            DbHelper.cs for shared connection/parameter code.
|
|-- BLL/                     Business Logic Layer: validation, auth, quiz
|                            scoring, the "is this actually allowed?" layer.
|
|-- Helpers/                 Small reusable utilities: PasswordHelper.cs,
|                            ErrorLogger.cs, SqlErrorHelper.cs.
|
|-- Masterpages/              Shared page layout: Site.master (public and
|                            student pages), Admin.master (admin section,
|                            where admin-only access is enforced).
|
|-- Pages/                   Public pages: Default.aspx (home),
|                            Courses.aspx (catalogue), CourseDetails.aspx,
|                            LessonDetails.aspx, Contact.aspx.
|
|-- Account/                 Login.aspx, Register.aspx, Logout.aspx,
|                            Profile.aspx.
|
|-- Member/                  Pages requiring login: Quiz.aspx,
|                            MyLearning.aspx.
|
|-- Admin/                   Pages requiring an Admin account: manage
|                            Categories, Courses, Lessons, Quizzes, Users,
|                            Feedback.
|
|-- Errors/                  Friendly 404 and 500 error pages.
|
|-- Content/                 site.css — the one stylesheet the whole site
|                            shares (plus any page-specific CSS files, kept
|                            to a minimum).
|
|-- App_Data/                Where ErrorLogger.cs writes errors.log.
|
|-- Web.config                Database connection string, session settings,
|                            error-page routing.
|
|-- Web.sitemap               Defines the breadcrumb trail on every page.
|
|-- Default.aspx              Project root redirect to Pages/Default.aspx.
|
|-- CreateDatabase.sql        Run once in SSMS to create the entire database.
|
`-- THIRD_PARTY_NOTICE.md      Credits the base template per its license
                              (see 01_AUDIT_AND_SANITIZE.md).
```

## Naming conventions (keep consistent with the base repo)

- One DAL class per table, named `{Table}DAL.cs`.
- One BLL class per entity, named `{Entity}BLL.cs`.
- Page files as `{Feature}.aspx` with matching `{Feature}.aspx.cs` and
  `{Feature}.aspx.designer.cs`.
- SQL parameters prefixed `@` and named exactly after the column they bind to.

## If a new feature seems to need a new folder

It almost never does. A new admin page goes in `Admin/`. A new small helper
goes in `Helpers/`. A new table's access code goes in `Data_Access_Layer/`
and `BLL/`. If something genuinely doesn't fit any existing folder, stop and
ask before creating one.
