/* ============================================================================
   Incremental update — adds failed-login tracking and temporary lockout.
   Run once in SSMS against your EXISTING LearningPlatformDB. Skip this if
   you're running the freshly-updated CreateDatabase.sql instead (it now
   creates these columns from the start) — don't run both.
   ========================================================================= */
USE LearningPlatformDB;
GO

IF COL_LENGTH('Users', 'FailedLoginAttempts') IS NULL
BEGIN
    ALTER TABLE Users ADD FailedLoginAttempts INT NOT NULL DEFAULT 0;
    PRINT 'Added Users.FailedLoginAttempts.';
END
ELSE
    PRINT 'Users.FailedLoginAttempts already exists — skipped.';
GO

IF COL_LENGTH('Users', 'LockoutEndUtc') IS NULL
BEGIN
    ALTER TABLE Users ADD LockoutEndUtc DATETIME2(0) NULL;
    PRINT 'Added Users.LockoutEndUtc.';
END
ELSE
    PRINT 'Users.LockoutEndUtc already exists — skipped.';
GO
