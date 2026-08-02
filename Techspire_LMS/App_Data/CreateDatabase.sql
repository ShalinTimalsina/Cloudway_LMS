/* ============================================================================
   Base Template — Generic Learning Platform Database
   Works as-is for: programming learning system, cybersecurity learning
   platform, or any similar "browse -> enrol -> learn -> quiz" system.
   Only the seed data at the bottom is domain-specific — replace it with
   your own categories/tags/courses.

   Target : SQL Server Express 2019/2022, run via SSMS
   Usage  : Open in SSMS, press F5. Safe to re-run - it drops and recreates.
   ========================================================================= */

USE master;
GO

IF DB_ID('LearningPlatformDB') IS NOT NULL
BEGIN
    ALTER DATABASE LearningPlatformDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LearningPlatformDB;
END
GO

CREATE DATABASE LearningPlatformDB;
GO

USE LearningPlatformDB;
GO

/* ============================================================================
   1. ROLES
   ========================================================================= */
CREATE TABLE Roles (
    RoleID      INT IDENTITY(1,1) NOT NULL,
    RoleName    NVARCHAR(30)  NOT NULL,
    Description NVARCHAR(200) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (RoleID),
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
);
GO

/* ============================================================================
   2. USERS
   ========================================================================= */
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(150) NOT NULL,
    PasswordHash CHAR(64)      NOT NULL,   -- SHA-256 hex, always 64 chars
    PasswordSalt CHAR(32)      NOT NULL,   -- 16 random bytes as hex
    RoleID       INT           NOT NULL,
    PhoneNumber  NVARCHAR(20)  NULL,
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2(0)  NOT NULL DEFAULT SYSUTCDATETIME(),
    LastLoginAt  DATETIME2(0)  NULL,
    CONSTRAINT PK_Users PRIMARY KEY (UserID),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO
CREATE INDEX IX_Users_RoleID ON Users(RoleID);
GO

/* ============================================================================
   3. CATEGORIES  (generic topic grouping — the data, not the schema, is domain-specific)
   ========================================================================= */
CREATE TABLE Categories (
    CategoryID   INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(60)  NOT NULL,
    Description  NVARCHAR(250) NULL,
    IsActive     BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_Categories PRIMARY KEY (CategoryID),
    CONSTRAINT UQ_Categories_Name UNIQUE (CategoryName)
);
GO

/* ============================================================================
   4. TAGS  (free-form M:N labelling — the extensibility hook for any domain)
   ========================================================================= */
CREATE TABLE Tags (
    TagID   INT IDENTITY(1,1) NOT NULL,
    TagName NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_Tags PRIMARY KEY (TagID),
    CONSTRAINT UQ_Tags_Name UNIQUE (TagName)
);
GO

/* ============================================================================
   5. COURSES
   ========================================================================= */
CREATE TABLE Courses (
    CourseID          INT IDENTITY(1,1) NOT NULL,
    Title             NVARCHAR(200)  NOT NULL,
    ShortDescription  NVARCHAR(300)  NULL,
    FullDescription   NVARCHAR(MAX)  NULL,
    CategoryID        INT            NOT NULL,
    ThumbnailPath     NVARCHAR(260)  NULL,
    DifficultyLevel   NVARCHAR(20)   NOT NULL DEFAULT 'Beginner',
    DurationMinutes   INT            NULL,
    IsPublished       BIT            NOT NULL DEFAULT 0,
    CreatedBy         INT            NOT NULL,
    CreatedAt         DATETIME2(0)   NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt         DATETIME2(0)   NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Courses PRIMARY KEY (CourseID),
    CONSTRAINT FK_Courses_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Courses_Users FOREIGN KEY (CreatedBy) REFERENCES Users(UserID),
    CONSTRAINT CK_Courses_Difficulty CHECK (DifficultyLevel IN ('Beginner','Intermediate','Advanced'))
);
GO
CREATE INDEX IX_Courses_CategoryID ON Courses(CategoryID);
CREATE INDEX IX_Courses_CreatedBy ON Courses(CreatedBy);
CREATE INDEX IX_Courses_IsPublished ON Courses(IsPublished);
GO

/* ============================================================================
   6. COURSETAGS  (junction table for Courses <-> Tags, M:N)
   ========================================================================= */
CREATE TABLE CourseTags (
    CourseID INT NOT NULL,
    TagID    INT NOT NULL,
    CONSTRAINT PK_CourseTags PRIMARY KEY (CourseID, TagID),
    CONSTRAINT FK_CourseTags_Courses FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE,
    CONSTRAINT FK_CourseTags_Tags FOREIGN KEY (TagID) REFERENCES Tags(TagID) ON DELETE CASCADE
);
GO

/* ============================================================================
   7. LESSONS
   ========================================================================= */
CREATE TABLE Lessons (
    LessonID        INT IDENTITY(1,1) NOT NULL,
    CourseID        INT           NOT NULL,
    Title           NVARCHAR(200) NOT NULL,
    ContentHtml     NVARCHAR(MAX) NULL,
    VideoUrl        NVARCHAR(500) NULL,
    SortOrder       INT           NOT NULL DEFAULT 1,
    DurationMinutes INT           NULL,
    CONSTRAINT PK_Lessons PRIMARY KEY (LessonID),
    CONSTRAINT FK_Lessons_Courses FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE
);
GO
CREATE INDEX IX_Lessons_CourseID ON Lessons(CourseID);
GO

/* ============================================================================
   8. RESOURCES  (generic downloadable attachment per lesson — code file, PCAP,
      slide deck, cheat sheet... same table, different ResourceType)
   ========================================================================= */
CREATE TABLE Resources (
    ResourceID   INT IDENTITY(1,1) NOT NULL,
    LessonID     INT           NOT NULL,
    Title        NVARCHAR(150) NOT NULL,
    FilePath     NVARCHAR(260) NOT NULL,
    ResourceType NVARCHAR(30)  NOT NULL DEFAULT 'Document',
    UploadedAt   DATETIME2(0)  NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Resources PRIMARY KEY (ResourceID),
    CONSTRAINT FK_Resources_Lessons FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID) ON DELETE CASCADE
);
GO
CREATE INDEX IX_Resources_LessonID ON Resources(LessonID);
GO

/* ============================================================================
   9. ENROLLMENTS  (junction table for Users <-> Courses, M:N, with attributes
      belonging to the relationship itself)
   ========================================================================= */
CREATE TABLE Enrollments (
    EnrollmentID    INT IDENTITY(1,1) NOT NULL,
    UserID          INT           NOT NULL,
    CourseID        INT           NOT NULL,
    EnrolledAt      DATETIME2(0)  NOT NULL DEFAULT SYSUTCDATETIME(),
    ProgressPercent DECIMAL(5,2)  NOT NULL DEFAULT 0,
    CompletedAt     DATETIME2(0)  NULL,
    CONSTRAINT PK_Enrollments PRIMARY KEY (EnrollmentID),
    CONSTRAINT FK_Enrollments_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
    CONSTRAINT UQ_Enrollments_UserCourse UNIQUE (UserID, CourseID)
);
GO
CREATE INDEX IX_Enrollments_CourseID ON Enrollments(CourseID);
GO

/* ============================================================================
   10. QUIZZES
   ========================================================================= */
CREATE TABLE Quizzes (
    QuizID   INT IDENTITY(1,1) NOT NULL,
    CourseID INT           NOT NULL,
    Title    NVARCHAR(150) NOT NULL,
    PassMark INT           NOT NULL DEFAULT 50,
    IsActive BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_Quizzes PRIMARY KEY (QuizID),
    CONSTRAINT FK_Quizzes_Courses FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE
);
GO
CREATE INDEX IX_Quizzes_CourseID ON Quizzes(CourseID);
GO

/* ============================================================================
   11. QUESTIONS  (fully normalised — no fixed OptionA..D, see QuestionOptions)
   ========================================================================= */
CREATE TABLE Questions (
    QuestionID   INT IDENTITY(1,1) NOT NULL,
    QuizID       INT           NOT NULL,
    QuestionText NVARCHAR(500) NOT NULL,
    QuestionType NVARCHAR(20)  NOT NULL DEFAULT 'SingleChoice',
    Marks        INT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_Questions PRIMARY KEY (QuestionID),
    CONSTRAINT FK_Questions_Quizzes FOREIGN KEY (QuizID) REFERENCES Quizzes(QuizID) ON DELETE CASCADE,
    CONSTRAINT CK_Questions_Type CHECK (QuestionType IN ('SingleChoice','MultipleChoice','TrueFalse'))
);
GO
CREATE INDEX IX_Questions_QuizID ON Questions(QuizID);
GO

/* ============================================================================
   12. QUESTIONOPTIONS  (any number of options, any number marked correct)
   ========================================================================= */
CREATE TABLE QuestionOptions (
    OptionID   INT IDENTITY(1,1) NOT NULL,
    QuestionID INT           NOT NULL,
    OptionText NVARCHAR(250) NOT NULL,
    IsCorrect  BIT           NOT NULL DEFAULT 0,
    SortOrder  INT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_QuestionOptions PRIMARY KEY (OptionID),
    CONSTRAINT FK_QuestionOptions_Questions FOREIGN KEY (QuestionID) REFERENCES Questions(QuestionID) ON DELETE CASCADE
);
GO
CREATE INDEX IX_QuestionOptions_QuestionID ON QuestionOptions(QuestionID);
GO

/* ============================================================================
   13. QUIZATTEMPTS
   ========================================================================= */
CREATE TABLE QuizAttempts (
    AttemptID   INT IDENTITY(1,1) NOT NULL,
    UserID      INT          NOT NULL,
    QuizID      INT          NOT NULL,
    Score       INT          NOT NULL,
    TotalMarks  INT          NOT NULL,  -- snapshot at attempt time
    IsPassed    BIT          NOT NULL,  -- computed against PassMark at attempt time
    AttemptedAt DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_QuizAttempts PRIMARY KEY (AttemptID),
    CONSTRAINT FK_QuizAttempts_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_QuizAttempts_Quizzes FOREIGN KEY (QuizID) REFERENCES Quizzes(QuizID)
);
GO
CREATE INDEX IX_QuizAttempts_UserID ON QuizAttempts(UserID);
CREATE INDEX IX_QuizAttempts_QuizID ON QuizAttempts(QuizID);
GO

/* ============================================================================
   14. FEEDBACK
   ========================================================================= */
CREATE TABLE Feedback (
    FeedbackID  INT IDENTITY(1,1) NOT NULL,
    UserID      INT           NULL,   -- NULL when a guest submits
    Name        NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(150) NOT NULL,
    Subject     NVARCHAR(150) NOT NULL,
    Message     NVARCHAR(MAX) NOT NULL,
    IsRead      BIT           NOT NULL DEFAULT 0,
    SubmittedAt DATETIME2(0)  NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Feedback PRIMARY KEY (FeedbackID),
    CONSTRAINT FK_Feedback_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

/* ============================================================================
   SEED DATA
   Everything below is the ONLY domain-specific part of this script.
   Replace it with your own category/tag/course names to retarget the
   template at a different subject (cybersecurity, networking, data
   science...). Table structure above never needs to change.
   ========================================================================= */

INSERT INTO Roles (RoleName, Description) VALUES
    ('Admin',  'Manages categories, courses, users and reviews feedback'),
    ('Member', 'Registered learner: enrols, studies, takes quizzes');
GO

-- Sample admin account. Password is placeholder text here; Phase 6 of the
-- teaching pack shows how to generate a real salted hash from C# and update
-- this row (or better, seed it from the app on first run).
INSERT INTO Users (FullName, Email, PasswordHash, PasswordSalt, RoleID, IsActive)
VALUES ('Platform Admin', 'admin@example.com',
        REPLICATE('0', 64), REPLICATE('0', 32), 1, 1);
GO

-- Example categories: swap these two lines for your own topic areas.
INSERT INTO Categories (CategoryName, Description) VALUES
    ('Getting Started', 'Foundational material for newcomers to the platform'),
    ('Core Skills',      'Intermediate material building on the basics');
GO

INSERT INTO Tags (TagName) VALUES
    ('Beginner-Friendly'), ('Hands-On'), ('Self-Paced');
GO

INSERT INTO Courses (Title, ShortDescription, CategoryID, DifficultyLevel, DurationMinutes, IsPublished, CreatedBy)
VALUES ('Sample Course — Replace Me',
        'A placeholder course showing the expected shape of real content.',
        1, 'Beginner', 60, 1, 1);
GO

INSERT INTO CourseTags (CourseID, TagID) VALUES (1, 1), (1, 2);
GO

INSERT INTO Lessons (CourseID, Title, ContentHtml, SortOrder, DurationMinutes) VALUES
    (1, 'Lesson 1 — Introduction', '<p>Replace with real lesson content.</p>', 1, 15),
    (1, 'Lesson 2 — Next Steps',   '<p>Replace with real lesson content.</p>', 2, 20);
GO

INSERT INTO Quizzes (CourseID, Title, PassMark) VALUES (1, 'Sample Course Quiz', 50);
GO

INSERT INTO Questions (QuizID, QuestionText, QuestionType, Marks) VALUES
    (1, 'Replace this with a real question.', 'SingleChoice', 1);
GO

INSERT INTO QuestionOptions (QuestionID, OptionText, IsCorrect, SortOrder) VALUES
    (1, 'Option A', 1, 1),
    (1, 'Option B', 0, 2),
    (1, 'Option C', 0, 3),
    (1, 'Option D', 0, 4);
GO

/* ============================================================================
   VERIFICATION QUERIES — run these after the script to confirm the build
   ========================================================================= */
SELECT * FROM Roles;
SELECT * FROM Users;
SELECT * FROM Categories;
SELECT c.CourseID, c.Title, cat.CategoryName, c.DifficultyLevel
FROM Courses c JOIN Categories cat ON c.CategoryID = cat.CategoryID;
SELECT l.Title AS Lesson, c.Title AS Course FROM Lessons l JOIN Courses c ON l.CourseID = c.CourseID;
SELECT q.QuestionText, o.OptionText, o.IsCorrect
FROM Questions q JOIN QuestionOptions o ON q.QuestionID = o.QuestionID;
GO
