using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class LessonDAL
    {
        public List<Lesson> SelectByCourse(int courseId)
        {
            List<Lesson> list = new List<Lesson>();
            const string sql = @"
                SELECT LessonID, CourseID, Title, Content, VideoUrl, OrderIndex
                FROM   Lessons
                WHERE  CourseID = @CourseID
                ORDER BY OrderIndex;";

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
                SELECT LessonID, CourseID, Title, Content, VideoUrl, OrderIndex
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
                INSERT INTO Lessons (CourseID, Title, Content, VideoUrl, OrderIndex)
                VALUES (@CourseID, @Title, @Content, @VideoUrl, @OrderIndex);
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
                SET Title = @Title, Content = @Content, VideoUrl = @VideoUrl,
                    OrderIndex = @OrderIndex = @DurationMinutes
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
            DbHelper.AddParam(cmd, "@Content", l.Content);
            DbHelper.AddParam(cmd, "@VideoUrl", l.VideoUrl);
            DbHelper.AddParam(cmd, "@OrderIndex", l.OrderIndex);
            
        }

        private Lesson Map(SqlDataReader r)
        {
            return new Lesson
            {
                LessonID        = DbHelper.GetInt(r, "LessonID"),
                CourseID        = DbHelper.GetInt(r, "CourseID"),
                Title           = DbHelper.GetString(r, "Title"),
                Content     = DbHelper.GetString(r, "Content"),
                VideoUrl        = DbHelper.GetString(r, "VideoUrl"),
                OrderIndex       = DbHelper.GetInt(r, "OrderIndex"),
                
            };
        }
    }
}
