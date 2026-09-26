# CloudWay — design guideline

The visual system only — architecture, schema and page functionality stay
in the other numbered files. This is what every page in the system draws
from: colours, type, spacing, components. Build `Content/site.css` directly
from this file.

## Design philosophy

A modern, AWS-console-inspired learning platform: dark slate-navy surfaces,
a warm amber accent, and a technical, no-nonsense feel — closer to the AWS
Management Console or AWS Skill Builder than to a generic course-marketplace
site. That association is intentional and appropriate for a platform
teaching AWS content. It stops short of being a copy: no AWS logo, no AWS
wordmark, and the accent colour is a distinct shade from AWS's own trademark
orange (see below) — the resemblance is in tone and layout convention, not
in reused brand assets.

Every screen answers "where am I, what can I do here, what happens if I
click this" at a glance. No decorative animation, no stock-photo hero
banners — content, status, and progress are the visual interest, the same
way the actual AWS console looks like a workspace, not a landing page.

## Colour palette

| Role | Colour | Hex | Usage |
|---|---|---|---|
| Primary / brand | Slate navy | `#152238` | Header, sidebar, nav background, primary buttons |
| Primary (darker, hover/pressed) | Deep slate | `#0E1826` | Button hover/active states |
| Accent | Amber | `#E8890C` | Links, active states, progress bars, CTAs, focus rings |
| Accent (light tint) | Pale amber | `#FDECD3` | Selected-row backgrounds, badge fills, hover highlights |
| Background | Cool light grey | `#F1F3F6` | Page background — mirrors a console's panel background, not pure white |
| Surface | White | `#FFFFFF` | Cards, forms, tables |
| Text primary | Near-black slate | `#161B22` | Body text, headings |
| Text secondary | Slate grey | `#5B6472` | Meta text, timestamps, helper text |
| Border | Light grey | `#DDE2E8` | Card borders, table dividers |
| Success | Green | `#1E8E5A` | Correct answer, "published" status |
| Danger | Red | `#D3453A` | Validation errors, delete actions, lockout messages |
| Warning | Gold | `#C98A00` | Low quiz score, near-lockout warning — deliberately a shade darker than the accent so it reads as "caution," not just "brand colour again" |

One accent (amber) does the heavy lifting across the whole site. Warning
gold is close to it on purpose — it should feel related to the AWS-service
visual language — but always paired with text/an icon, never colour alone,
so it's never ambiguous with a plain accent element.

## Typography

Readable first — nothing on this site should feel like fine print.

- **Headings:** "Poppins" (Google Fonts), weight 600. Confident, not
  decorative — no weight above 600 anywhere, no all-caps headings.
- **Body:** "Inter" (Google Fonts), weight 400, 500 for emphasis.
- **Monospace accent (new):** "JetBrains Mono" (Google Fonts), used only
  for short AWS service references and status codes — e.g. a small
  `EC2` / `S3` / `IAM` tag inside a course card, or a quiz result code. Never
  used for body copy or headings. This is the one deliberate "console" detail
  that sells the AWS-platform feel without needing icons or imagery.

**Scale (revised — larger and more distinct steps than a typical default):**

| Level | Size | Weight | Line-height | Use |
|---|---|---|---|---|
| h1 | 34px | 600 | 1.25 | Page titles only, one per page |
| h2 | 26px | 600 | 1.3 | Section headings |
| h3 | 20px | 600 | 1.35 | Card titles, sub-sections |
| Body | 17px | 400 | 1.6 | All paragraph and form-label text |
| Body emphasis | 17px | 500 | 1.6 | Important inline text, button labels |
| Meta / small | 14px | 400 | 1.5 | Timestamps, helper text, badge labels — **never smaller than 14px anywhere in the system**, including mobile |

Why bigger than a typical default: 16px/13px scales read fine on a
marketing site skimmed for ten seconds, not on a *study* platform where
someone reads paragraphs of lesson content for several minutes at a time.
17px body with 1.6 line-height is the floor here, not the ceiling — do not
shrink it responsively. On mobile, only h1 scales down (34px → 28px); body
and meta text stay fixed at 17px/14px at every breakpoint, and form inputs
stay at 17px specifically so iOS doesn't auto-zoom on focus.

Load all three families via `<link>` to `fonts.googleapis.com` in both
`Site.master`'s and `Admin.master`'s `<head>` — distinct from the "external
CSS" rubric item, which is `site.css` itself.

## Layout and spacing

- Base spacing unit: 8px. All margin/padding in multiples of it (8, 16, 24,
  32, 48) — no one-off values.
- Max content width: 1140px, centred, 24px side padding on smaller screens.
- Grid: CSS Grid/Flexbox for card layouts — no framework grid needed at this
  scale.
- Breakpoints: 960px (tablet), 640px (mobile). Nav collapses to a stacked
  menu below 640px — one JS toggle, no framework.
- Card corner radius: 8px. Buttons: 6px. Consistent everywhere.
- Interactive elements (buttons, inputs, per-row admin actions) are 44px
  tall minimum — a real touch-target size, not just a font-fit size.

## Icons

A handful of small inline SVGs (checkmark, chevron, lock, user) — no icon
font library. Keep each icon under ~10 path points; if it needs more detail
than that, it's the wrong icon for this system.

## CSS architecture (satisfies the explicit rubric line)

- **External:** `Content/site.css` — colour variables (`:root { --primary:
  #152238; --accent: #E8890C; ... }`), typography, layout, every component
  below. The vast majority of the CSS lives here.
- **Internal:** one page-specific `<style>` block, on `LessonDetails.aspx`,
  scoping rules for the rendered lesson HTML content (`<h3>`, `<ul>`, etc.
  per `03_DATA_MODEL.md`'s content decision).
- **Inline:** one deliberate `style="..."` on the progress bar's fill width
  (`style="width: 62%"`) — genuinely dynamic per-enrolment data, honestly
  needs to be inline rather than a static class.

## Core components

- **Button** — primary (slate-navy bg, white text, amber on hover-outline),
  secondary (white bg, navy border/text), danger (red, delete actions). 44px
  height, 12px 20px padding, 6px radius, 17px/500 weight label — consistent
  across all three variants.
- **Card** — white surface, 1px light-grey border, 8px radius, 16–24px
  padding. Course tiles, lesson list items, admin summary tiles.
- **Service tag** (new) — a tiny pill in the monospace font, pale-amber
  background, navy text — e.g. `EC2`, `S3`, `IAM` — attached to course cards
  to show which AWS service a course covers.
- **Form field** — label above input (17px), 44px input height, 6px radius,
  grey border → amber border on focus, red border + red 14px helper text on
  validation failure (mirrors the existing server-side `ValidationException`
  messages — no new UI-layer message strings invented).
- **Table** (admin grids) — white surface, light-grey row dividers, no
  vertical lines, pale-amber row hover, right-aligned edit/delete icons.
- **Badge** — small pill for role (Admin/Student), publish status
  (Published/Draft), quiz pass/fail. Colour + text together, always.
- **Progress bar** — light-grey track, amber fill, percentage label beside
  it (the one inline-style use case above).
- **Alert/banner** — success (green-tinted bg), error (red-tinted bg), used
  for form feedback and the lockout/attempts messaging from the auth UX
  enhancements.

## Navigation structure

- **Guest nav:** Home · Courses · Contact · Login · Register
- **Student nav (post-login):** Home · Courses · My Learning · [name/role
  badge, dropdown: Profile, Logout]
- **Admin nav:** separate `Admin.master` layout — dark slate-navy sidebar
  (Categories, Courses, Lessons, Quizzes, Users, Feedback) on a light-grey
  content background, console-style; top bar shows [Admin name, Logout].
  Admins reach the public/student nav via a small "View site" link (an
  Admin account is still a normal Student underneath, per
  `Project_documentation.pdf`).
- Breadcrumbs on every non-home page, sourced from `Web.sitemap` as in the
  base repo — restyled only, mechanism unchanged.
