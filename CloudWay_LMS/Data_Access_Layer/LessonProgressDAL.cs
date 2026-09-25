using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class LessonProgressDAL : PersistentConnection
    {

       
            public bool IsComplete(int enrollmentId, int lessonId)
            {
                const string sql = "SELECT COUNT(*) FROM LessonProgress WHERE EnrollmentID = @EnrollmentID AND LessonID = @LessonID;";

                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
                {
                    DbHelper.AddParam(cmd, "@EnrollmentID", enrollmentId);
                    DbHelper.AddParam(cmd, "@LessonID", lessonId);
                    con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }

            /// <summary>All completed LessonIDs for one enrollment — one query,
            /// used to checkmark a course's whole lesson list in a single pass
            /// instead of one IsComplete() call per lesson.</summary>
            public List<int> SelectCompletedLessonIds(int enrollmentId)
            {
                List<int> ids = new List<int>();
                const string sql = "SELECT LessonID FROM LessonProgress WHERE EnrollmentID = @EnrollmentID;";

                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
                {
                    DbHelper.AddParam(cmd, "@EnrollmentID", enrollmentId);
                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read()) ids.Add(DbHelper.GetInt(r, "LessonID"));
                    }
                }
                return ids;
            }

            public int CountCompleted(int enrollmentId)
            {
                const string sql = "SELECT COUNT(*) FROM LessonProgress WHERE EnrollmentID = @EnrollmentID;";

                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
                {
                    DbHelper.AddParam(cmd, "@EnrollmentID", enrollmentId);
                    con.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }

            /// <summary>Insert-if-not-already-there. The UNIQUE(EnrollmentID, LessonID)
            /// constraint means a duplicate click races safely even without the
            /// IsComplete() check above — this just avoids throwing in the normal case.</summary>
            public void MarkComplete(int enrollmentId, int lessonId)
            {
                if (IsComplete(enrollmentId, lessonId)) return;

                const string sql = @"
                INSERT INTO LessonProgress (EnrollmentID, LessonID, CompletedAt)
                VALUES (@EnrollmentID, @LessonID, SYSUTCDATETIME());";

                using (SqlConnection con = DbHelper.GetConnection())
                using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
                {
                    DbHelper.AddParam(cmd, "@EnrollmentID", enrollmentId);
                    DbHelper.AddParam(cmd, "@LessonID", lessonId);
                    con.Open();
                    try { cmd.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        // Two near-simultaneous clicks both passed the IsComplete()
                        // check before either committed — the unique constraint
                        // catches it, and the second one is a harmless no-op.
                        if (ex.Number != 2627 && ex.Number != 2601) throw;
                    }
                }
            }
        }
    
}
