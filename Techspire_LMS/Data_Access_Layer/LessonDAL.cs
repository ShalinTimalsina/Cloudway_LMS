using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    public class LessonDAL
    {
        public List<Lesson> SelectByCourse(int courseId)
        {
            List<Lesson> list = new List<Lesson>();
            const string sql = @"
                SELECT LessonID, CourseID, Title, ContentHtml, VideoUrl, SortOrder, DurationMinutes
                FROM   Lessons
                WHERE  CourseID = @CourseID
                ORDER BY SortOrder;";

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

        public Lesson SelectById(int lessonId)
        {
            const string sql = @"
                SELECT LessonID, CourseID, Title, ContentHtml, VideoUrl, SortOrder, DurationMinutes
                FROM   Lessons WHERE LessonID = @LessonID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@LessonID", lessonId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public int Insert(Lesson l)
        {
            const string sql = @"
                INSERT INTO Lessons (CourseID, Title, ContentHtml, VideoUrl, SortOrder, DurationMinutes)
                VALUES (@CourseID, @Title, @ContentHtml, @VideoUrl, @SortOrder, @DurationMinutes);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                AddParams(cmd, l);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Lesson l)
        {
            const string sql = @"
                UPDATE Lessons
                SET Title = @Title, ContentHtml = @ContentHtml, VideoUrl = @VideoUrl,
                    SortOrder = @SortOrder, DurationMinutes = @DurationMinutes
                WHERE LessonID = @LessonID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                AddParams(cmd, l);
                DbHelper.AddParam(cmd, "@LessonID", l.LessonID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int lessonId)
        {
            const string sql = "DELETE FROM Lessons WHERE LessonID = @LessonID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@LessonID", lessonId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private void AddParams(SqlCommand cmd, Lesson l)
        {
            DbHelper.AddParam(cmd, "@CourseID", l.CourseID);
            DbHelper.AddParam(cmd, "@Title", l.Title);
            DbHelper.AddParam(cmd, "@ContentHtml", l.ContentHtml);
            DbHelper.AddParam(cmd, "@VideoUrl", l.VideoUrl);
            DbHelper.AddParam(cmd, "@SortOrder", l.SortOrder);
            DbHelper.AddParam(cmd, "@DurationMinutes", l.DurationMinutes);
        }

        private Lesson Map(SqlDataReader r)
        {
            return new Lesson
            {
                LessonID        = DbHelper.GetInt(r, "LessonID"),
                CourseID        = DbHelper.GetInt(r, "CourseID"),
                Title           = DbHelper.GetString(r, "Title"),
                ContentHtml     = DbHelper.GetString(r, "ContentHtml"),
                VideoUrl        = DbHelper.GetString(r, "VideoUrl"),
                SortOrder       = DbHelper.GetInt(r, "SortOrder"),
                DurationMinutes = DbHelper.GetNullableInt(r, "DurationMinutes")
            };
        }
    }
}
