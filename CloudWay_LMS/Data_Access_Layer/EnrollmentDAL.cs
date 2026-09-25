using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    /// <summary>The Users<->Courses junction table, plus attributes that belong
    /// to the relationship itself (EnrolledAt, ProgressPercent, CompletedAt).</summary>
    public class EnrollmentDAL
    {
        public List<Enrollment> SelectByUser(int userId)
        {
            List<Enrollment> list = new List<Enrollment>();
            const string sql = @"
                SELECT e.EnrollmentID, e.UserID, e.CourseID, e.EnrolledAt, e.ProgressPercent, e.
                       c.Title AS CourseTitle, c.ThumbnailPath
                FROM   Enrollments e JOIN Courses c ON e.CourseID = c.CourseID
                WHERE  e.UserID = @UserID
                ORDER BY e.EnrolledAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public List<Enrollment> SelectByCourse(int courseId)
        {
            List<Enrollment> list = new List<Enrollment>();
            const string sql = @"
                SELECT EnrollmentID, UserID, CourseID, EnrolledAt, ProgressPercent, CompletedAt
                FROM   Enrollments
                WHERE  CourseID = @CourseID
                ORDER BY EnrolledAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public bool IsEnrolled(int userId, int courseId)
        {
            const string sql = "SELECT COUNT(*) FROM Enrollments WHERE UserID = @UserID AND CourseID = @CourseID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        /// <summary>Insert; relies on the UNIQUE(UserID, CourseID) constraint to stop
        /// double enrolment at the database level even if a bug lets two requests
        /// race the BLL's IsEnrolled check — catch SqlException 2627 for a friendly message.</summary>
        public int Insert(int userId, int courseId)
        {
            const string sql = @"
                INSERT INTO Enrollments (UserID, CourseID, EnrolledAt, ProgressPercent)
                VALUES (@UserID, @CourseID, SYSUTCDATETIME(), 0);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool UpdateProgress(int enrollmentId, decimal progressPercent, bool markCompleted)
        {
            const string sql = @"
                UPDATE Enrollments
                SET ProgressPercent = @ProgressPercent,
                    CompletedAt = CASE WHEN @MarkCompleted = 1 THEN SYSUTCDATETIME() ELSE CompletedAt END
                WHERE EnrollmentID = @EnrollmentID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@ProgressPercent", progressPercent);
                DbHelper.AddParam(cmd, "@MarkCompleted", markCompleted);
                DbHelper.AddParam(cmd, "@EnrollmentID", enrollmentId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int enrollmentId)
        {
            const string sql = "DELETE FROM Enrollments WHERE EnrollmentID = @EnrollmentID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@EnrollmentID", enrollmentId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Enrollment Map(SqlDataReader r)
        {
            var e = new Enrollment
            {
                EnrollmentID    = DbHelper.GetInt(r, "EnrollmentID"),
                UserID          = DbHelper.GetInt(r, "UserID"),
                CourseID        = DbHelper.GetInt(r, "CourseID"),
                EnrolledAt      = DbHelper.GetDate(r, "EnrolledAt"),
                ProgressPercent = DbHelper.GetDecimal(r, "ProgressPercent")
            };
            if (HasColumn(r, "CourseTitle")) e.CourseTitle = DbHelper.GetString(r, "CourseTitle");
            if (HasColumn(r, "ThumbnailPath")) e.ThumbnailPath = DbHelper.GetString(r, "ThumbnailPath");
            return e;
        }

        private bool HasColumn(IDataRecord r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (r.GetName(i).Equals(col)) return true;
            return false;
        }
    }
}
