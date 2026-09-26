# CloudWay content outline

Five courses, one per AWS domain category from `03_DATA_MODEL.md`. Each
course file in this folder follows your exact template (Course Info,
Lessons, Quiz Settings, Quiz Questions) and is ready to seed directly.

| # | Category | Course | Lessons | Quiz Qs | File |
|---|---|---|---|---|---|
| 1 | Compute | Amazon EC2 Fundamentals | 5 | 5 | `01_ec2-fundamentals.md` |
| 2 | Storage | Amazon S3 Essentials | 5 | 5 | `02_s3-essentials.md` |
| 3 | Networking | Amazon VPC Fundamentals | 5 | 5 | `03_vpc-fundamentals.md` |
| 4 | Security & IAM | AWS IAM Fundamentals | 5 | 5 | `04_iam-fundamentals.md` |
| 5 | Databases | Amazon RDS Fundamentals | 5 | 5 | `05_rds-fundamentals.md` |

25 lessons, 25 quiz questions total — enough to exercise every CRUD path,
every Category/Course/Lesson/Quiz relationship in `03_DATA_MODEL.md`, and a
real, varied "My Learning" quiz-history dashboard once a test student
attempts a few of them.

## Tags (per `03_DATA_MODEL.md`'s Tags/CourseTags junction)

All five courses are tagged with the certification track(s) they're
relevant to:

- **Cloud Practitioner** — applied to all five (every topic here is
  foundational, CLF-C02-level material)
- **Solutions Architect Associate** — applied to EC2, S3, VPC, RDS (the
  four services most heavily tested on the SAA-C03 exam)
- Course-specific service tags: `EC2`, `S3`, `VPC`, `IAM`, `RDS` (one per
  course, matching the "service tag" component in `07_DESIGN.md`)

## Video URLs — intentionally left blank

None of these lessons carry a Video URL. I ran real searches while
preparing an earlier course and found that reliable, verifiable
lesson-specific AWS video links are hard to source without risking a dead
or mismatched link — and a wrong link is worse than no link. Your
template explicitly allows leaving this field blank, so every lesson here
does, consistent with the text-only decision in `03_DATA_MODEL.md`. If your
team wants to add specific videos later, verify each one directly on
YouTube before pasting it in — I'm glad to do that verification one link at
a time if you want to add them.

## Passing score convention

Every quiz uses an 80% passing score, matching the target grade convention
you set for the assignment itself — a small, deliberate consistency touch,
not a technical requirement.
