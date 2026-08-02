using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    public class QuizDAL
    {
        public List<Quiz> SelectByCourse(int courseId)
        {
            List<Quiz> list = new List<Quiz>();
            const string sql = @"
                SELECT q.QuizID, q.CourseID, q.Title, q.PassMark, q.IsActive,
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
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public Quiz SelectById(int quizId)
        {
            const string sql = "SELECT QuizID, CourseID, Title, PassMark, IsActive FROM Quizzes WHERE QuizID = @QuizID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", quizId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public int Insert(Quiz q)
        {
            const string sql = @"
                INSERT INTO Quizzes (CourseID, Title, PassMark, IsActive)
                VALUES (@CourseID, @Title, @PassMark, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", q.CourseID);
                DbHelper.AddParam(cmd, "@Title", q.Title);
                DbHelper.AddParam(cmd, "@PassMark", q.PassMark);
                DbHelper.AddParam(cmd, "@IsActive", q.IsActive);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Quiz q)
        {
            const string sql = @"
                UPDATE Quizzes SET Title = @Title, PassMark = @PassMark, IsActive = @IsActive
                WHERE QuizID = @QuizID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@Title", q.Title);
                DbHelper.AddParam(cmd, "@PassMark", q.PassMark);
                DbHelper.AddParam(cmd, "@IsActive", q.IsActive);
                DbHelper.AddParam(cmd, "@QuizID", q.QuizID);
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

        private Quiz Map(SqlDataReader r)
        {
            return new Quiz
            {
                QuizID   = DbHelper.GetInt(r, "QuizID"),
                CourseID = DbHelper.GetInt(r, "CourseID"),
                Title    = DbHelper.GetString(r, "Title"),
                PassMark = DbHelper.GetInt(r, "PassMark"),
                IsActive = DbHelper.GetBool(r, "IsActive")
            };
        }
    }
}
