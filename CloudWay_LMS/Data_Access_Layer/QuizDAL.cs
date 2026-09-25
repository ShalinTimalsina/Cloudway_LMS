using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class QuizDAL
    {
        public List<Quiz> SelectByCourse(int courseId)
        {
            List<Quiz> list = new List<Quiz>();
            const string sql = @"
                SELECT q.QuizID, q.CourseID, q.Title, q.PassingScore, q.Description, q.TimeLimitMinutes, q.IsPublished,
                       (SELECT COUNT(*) FROM Questions qq WHERE qq.QuizID = q.QuizID) AS QuestionCount
                FROM   Quizzes q
                WHERE  q.CourseID = @CourseID
                ORDER BY q.QuizID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Quiz q = Map(r);
                        q.QuestionCount = DbHelper.GetInt(r, "QuestionCount");
                        list.Add(q);
                    }
                }
            }
            return list;
        }

        public Quiz SelectById(int quizId)
        {
            const string sql = "SELECT QuizID, CourseID, Title, PassingScore, Description, TimeLimitMinutes, IsPublished FROM Quizzes WHERE QuizID = @QuizID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", quizId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public int Insert(Quiz q)
        {
            const string sql = @"
                INSERT INTO Quizzes (CourseID, Title, PassingScore, Description, TimeLimitMinutes, IsPublished)
                VALUES (@CourseID, @Title, @PassingScore, @Description, @TimeLimitMinutes, @IsPublished);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", q.CourseID);
                DbHelper.AddParam(cmd, "@Title", q.Title);
                DbHelper.AddParam(cmd, "@PassingScore", q.PassingScore);
                DbHelper.AddParam(cmd, "@Description", q.Description);
                DbHelper.AddParam(cmd, "@TimeLimitMinutes", (object)q.TimeLimitMinutes ?? System.DBNull.Value);
                DbHelper.AddParam(cmd, "@IsPublished", q.IsPublished);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Quiz q)
        {
            const string sql = @"
                UPDATE Quizzes
                SET    Title = @Title, PassingScore = @PassingScore, Description = @Description, TimeLimitMinutes = @TimeLimitMinutes, IsPublished = @IsPublished
                WHERE  QuizID = @QuizID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", q.QuizID);
                DbHelper.AddParam(cmd, "@Title", q.Title);
                DbHelper.AddParam(cmd, "@PassingScore", q.PassingScore);
                DbHelper.AddParam(cmd, "@Description", q.Description);
                DbHelper.AddParam(cmd, "@TimeLimitMinutes", (object)q.TimeLimitMinutes ?? System.DBNull.Value);
                DbHelper.AddParam(cmd, "@IsPublished", q.IsPublished);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int quizId)
        {
            const string sql = "DELETE FROM Quizzes WHERE QuizID = @QuizID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", quizId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool HasAttempts(int quizId)
        {
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM QuizAttempts WHERE QuizID = @QuizID) THEN 1 ELSE 0 END;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", quizId);
                con.Open();
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

        private Quiz Map(SqlDataReader r)
        {
            return new Quiz
            {
                QuizID = DbHelper.GetInt(r, "QuizID"),
                CourseID = DbHelper.GetInt(r, "CourseID"),
                Title = DbHelper.GetString(r, "Title"),
                PassingScore = DbHelper.GetInt(r, "PassingScore"),
                Description = DbHelper.GetString(r, "Description"),
                TimeLimitMinutes = DbHelper.GetNullableInt(r, "TimeLimitMinutes"),
                IsPublished = DbHelper.GetBool(r, "IsPublished")
            };
        }
    }
}
