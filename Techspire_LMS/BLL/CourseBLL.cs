using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Helpers;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    /// <summary>
    /// Business rules for courses. Knows nothing about TextBoxes and nothing
    /// about SQL syntax — this class could be reused unchanged in a desktop
    /// app or a console importer, which is the test for honest layering.
    /// </summary>
    public class CourseBLL
    {
        private readonly CourseDAL _dal = new CourseDAL();
        private static readonly string[] AllowedDifficulties = { "Beginner", "Intermediate", "Advanced" };

        // ---------------- Reads ----------------

        public List<Course> GetFeatured(int count)
        {
            if (count <= 0) count = 6;
            if (count > 50) count = 50; // defensive clamp — never trust a caller's number straight into SQL TOP
            return _dal.SelectFeatured(count);
        }

        public List<Course> GetPublic(int? categoryId, int? tagId, string search)
        {
            return _dal.SelectPublished(categoryId, tagId, search);
        }

        public List<Course> GetAllForAdmin() { return _dal.SelectAll(); }

        public Course GetById(int courseId)
        {
            if (courseId <= 0) return null;
            return _dal.SelectById(courseId);
        }

        /// <summary>A guest may only see a PUBLISHED course. Returning null for a
        /// draft makes the details page show "not found" rather than leaking
        /// the existence of unreleased content.</summary>
        public Course GetPublishedById(int courseId)
        {
            Course c = GetById(courseId);
            return (c != null && c.IsPublished) ? c : null;
        }

        // ---------------- Writes ----------------

        public int Add(Course c, bool callerIsAdmin)
        {
            Validate(c);

            if (!callerIsAdmin && c.IsPublished)
                throw new ValidationException("Only an administrator may publish a course.");

            try
            {
                return _dal.Insert(c);
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, fkMessage: "The selected category does not exist. Refresh the page and try again.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "CourseBLL.Add");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        public void Edit(Course c, bool callerIsAdmin)
        {
            if (c == null || c.CourseID <= 0) throw new ValidationException("Invalid course.");
            Validate(c);

            if (!callerIsAdmin && c.IsPublished)
                throw new ValidationException("Only an administrator may publish a course.");

            try
            {
                if (!_dal.Update(c))
                    throw new ValidationException("The course no longer exists. It may have been deleted.");
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, fkMessage: "The selected category does not exist. Refresh the page and try again.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "CourseBLL.Edit");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        /// <summary>
        /// Lessons/Resources/Quizzes/Questions/QuestionOptions cascade away
        /// automatically (see CreateDatabase.sql). Enrollments deliberately do
        /// NOT cascade, so a course with active learners hits the FK (error
        /// 547) and the admin gets told why instead of a raw SQL error.
        /// </summary>
        public void Remove(int courseId)
        {
            if (courseId <= 0) throw new ValidationException("Invalid course.");

            try
            {
                if (!_dal.Delete(courseId))
                    throw new ValidationException("The course no longer exists.");
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, fkMessage: "This course still has learners enrolled and cannot be deleted. Unpublish it instead.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "CourseBLL.Remove");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        // ---------------- Rules ----------------

        /// <summary>SERVER-SIDE validation. Page-level validator controls help the
        /// user, but can be bypassed (JS disabled, direct POST) — this method is
        /// the one that actually protects the data.</summary>
        private void Validate(Course c)
        {
            if (c == null) throw new ValidationException("No course data was supplied.");

            if (string.IsNullOrWhiteSpace(c.Title)) throw new ValidationException("Title is required.");
            c.Title = c.Title.Trim();
            if (c.Title.Length > 200) throw new ValidationException("Title must be 200 characters or fewer.");

            if (c.CategoryID <= 0) throw new ValidationException("Please choose a category.");

            if (c.DurationMinutes.HasValue && c.DurationMinutes.Value <= 0)
                throw new ValidationException("Duration must be greater than zero.");

            if (Array.IndexOf(AllowedDifficulties, c.DifficultyLevel) < 0)
                throw new ValidationException("Difficulty must be Beginner, Intermediate or Advanced.");

            if (!string.IsNullOrEmpty(c.ShortDescription) && c.ShortDescription.Length > 300)
                throw new ValidationException("Short description must be 300 characters or fewer.");
        }
    }
}
