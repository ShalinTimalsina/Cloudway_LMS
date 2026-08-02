using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Helpers;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    public class EnrollmentBLL
    {
        private readonly EnrollmentDAL _dal = new EnrollmentDAL();
        private readonly CourseDAL _courseDal = new CourseDAL();

        public List<Enrollment> GetByUser(int userId) { return _dal.SelectByUser(userId); }
        public List<Enrollment> GetByCourse(int courseId) { return _dal.SelectByCourse(courseId); }

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

        public void Unenroll(int enrollmentId)
        {
            if (enrollmentId <= 0) throw new ValidationException("Invalid enrolment.");
            if (!_dal.Delete(enrollmentId)) throw new ValidationException("The enrolment no longer exists.");
        }
    }
}
