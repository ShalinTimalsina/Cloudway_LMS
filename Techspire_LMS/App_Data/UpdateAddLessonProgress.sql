/* ============================================================================
   Incremental update — adds per-lesson completion tracking.
   Run this once in SSMS against your EXISTING LearningPlatformDB. If you
   haven't created the database yet, just run CreateDatabase.sql (updated
   to include this table) instead — don't run both.
   ========================================================================= */
USE LearningPlatformDB;
GO

IF OBJECT_ID('LessonProgress', 'U') IS NOT NULL
BEGIN
    PRINT 'LessonProgress already exists — nothing to do.';
END
ELSE
BEGIN
    CREATE TABLE LessonProgress (
        LessonProgressID INT IDENTITY(1,1) NOT NULL,
        EnrollmentID      INT           NOT NULL,
        LessonID          INT           NOT NULL,
        CompletedAt       DATETIME2(0)  NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_LessonProgress PRIMARY KEY (LessonProgressID),
        CONSTRAINT FK_LessonProgress_Enrollments FOREIGN KEY (EnrollmentID)
            REFERENCES Enrollments(EnrollmentID) ON DELETE CASCADE,
        CONSTRAINT FK_LessonProgress_Lessons FOREIGN KEY (LessonID)
            REFERENCES Lessons(LessonID) ON DELETE CASCADE,
        -- One completion record per lesson per enrollment — clicking
        -- "mark complete" twice on the same lesson is a no-op, not a
        -- duplicate row skewing the progress percentage.
        CONSTRAINT UQ_LessonProgress_EnrollmentLesson UNIQUE (EnrollmentID, LessonID)
    );

    CREATE INDEX IX_LessonProgress_EnrollmentID ON LessonProgress(EnrollmentID);

    PRINT 'LessonProgress table created.';
END
GO
