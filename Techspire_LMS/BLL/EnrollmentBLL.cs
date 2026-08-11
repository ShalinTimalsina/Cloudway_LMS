using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Helpers;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    public class EnrollmentBLL
    {
        private readonly EnrollmentDAL _dal = new EnrollmentDAL();
        private readonly CourseDAL _courseDal = new CourseDAL();
        private readonly LessonDAL _lessonDal = new LessonDAL();
        private readonly LessonProgressDAL _progressDal = new LessonProgressDAL();

        public List<Enrollment> GetByUser(int userId) { return _dal.SelectByUser(userId); }
        public List<Enrollment> GetByCourse(int courseId) { return _dal.SelectByCourse(courseId); }

        /// <summary>The enrollment record for this user+course, or null if
        /// they aren't enrolled — the lookup every lesson-progress method
        /// below needs first, since LessonProgress is keyed by EnrollmentID,
        /// not directly by UserID+CourseID.</summary>
        public Enrollment GetEnrollment(int userId, int courseId)
        {
            if (userId <= 0 || courseId <= 0) return null;
            return _dal.SelectByUser(userId).FirstOrDefault(e => e.CourseID == courseId);
        }

        /// <summary>
        /// Checks eligibility (course exists, is published, learner isn't
        /// already enrolled) BEFORE hitting the database, so the normal path
        /// never has to rely on catching a constraint violation. The
        /// UNIQUE(UserID, CourseID) constraint in the schema is still the
        /// real safety net for a race between two simultaneous requests —
        /// the catch block below handles that rare case with the same
        /// friendly message.
        /// </summary>
        public int Enroll(int userId, int courseId)
        {
            if (userId <= 0) throw new ValidationException("You must be logged in to enrol.");

            Course course = _courseDal.SelectById(courseId);
            if (course == null || !course.IsPublished)
                throw new ValidationException("This course is not available for enrolment.");

            if (_dal.IsEnrolled(userId, courseId))
                throw new ValidationException("You are already enrolled in this course.");

            try
            {
                return _dal.Insert(userId, courseId);
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, uniqueMessage: "You are already enrolled in this course.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "EnrollmentBLL.Enroll");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        public void UpdateProgress(int enrollmentId, decimal progressPercent)
        {
            if (enrollmentId <= 0) throw new ValidationException("Invalid enrolment.");
            if (progressPercent < 0 || progressPercent > 100)
                throw new ValidationException("Progress must be between 0 and 100.");

            bool markCompleted = progressPercent >= 100;
            if (!_dal.UpdateProgress(enrollmentId, progressPercent, markCompleted))
                throw new ValidationException("The enrolment no longer exists.");
        }

        /// <summary>
        /// userId is REQUIRED and checked — never trust EnrollmentID alone.
        /// It arrives at this method from a page's CommandArgument, which is
        /// client-controlled data exactly like a query-string value: a
        /// user could tamper with it to try unenrolling someone else. This
        /// method confirms the enrollment actually belongs to the calling
        /// user before deleting anything — the same IDOR discipline as
        /// CourseDetails.aspx's enrol action (identity from Session,
        /// ownership verified server-side, never assumed from what the
        /// client sent).
        /// </summary>
        public void Unenroll(int userId, int enrollmentId)
        {
            if (enrollmentId <= 0) throw new ValidationException("Invalid enrolment.");

            Enrollment enrollment = _dal.SelectByUser(userId).FirstOrDefault(e => e.EnrollmentID == enrollmentId);
            if (enrollment == null)
                throw new ValidationException("This enrolment doesn't belong to you, or no longer exists.");

            if (!_dal.Delete(enrollmentId)) throw new ValidationException("The enrolment no longer exists.");
        }

        // ====================================================================
        // Per-lesson completion — what LessonDetails.aspx and MyLearning.aspx
        // actually run on.
        // ====================================================================

        public List<int> GetCompletedLessonIds(int enrollmentId)
        {
            return _progressDal.SelectCompletedLessonIds(enrollmentId);
        }

        public bool IsLessonComplete(int enrollmentId, int lessonId)
        {
            return _progressDal.IsComplete(enrollmentId, lessonId);
        }

        /// <summary>
        /// Marks one lesson complete, then recomputes ProgressPercent from
        /// scratch as (completed lessons / total lessons in the course) *
        /// 100 — never incremented by a fixed step. Recomputing from the
        /// actual LessonProgress rows every time means the percentage stays
        /// correct even if lessons are later added to or removed from the
        /// course, and calling this twice for the same lesson is harmless
        /// (MarkComplete is a no-op on a repeat, so the recompute just
        /// produces the same number again).
        /// </summary>
        public decimal MarkLessonComplete(int userId, int courseId, int lessonId)
        {
            Enrollment enrollment = GetEnrollment(userId, courseId);
            if (enrollment == null)
                throw new ValidationException("You must be enrolled in this course to track progress.");

            _progressDal.MarkComplete(enrollment.EnrollmentID, lessonId);

            int totalLessons = _lessonDal.SelectByCourse(courseId).Count;
            int completedLessons = _progressDal.CountCompleted(enrollment.EnrollmentID);

            decimal newProgress = totalLessons == 0
                ? 0
                : Math.Round((decimal)completedLessons * 100 / totalLessons, 0);

            UpdateProgress(enrollment.EnrollmentID, newProgress);
            return newProgress;
        }
    }
}