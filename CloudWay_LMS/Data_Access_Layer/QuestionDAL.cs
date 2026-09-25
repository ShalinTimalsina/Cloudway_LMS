using System.Collections.Generic;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    /// <summary>SQL for Questions only. Options come from QuestionOptionDAL —
    /// call BLL.GetQuizWithQuestions (or load both DALs from the page) to
    /// assemble a full quiz for display.</summary>
    public class QuestionDAL
    {
        public List<Question> SelectByQuiz(int quizId)
        {
            List<Question> list = new List<Question>();
            const string sql = @"
                SELECT QuestionID, QuizID, QuestionText, QuestionType, Marks
                FROM   Questions
                WHERE  QuizID = @QuizID
                ORDER BY QuestionID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", quizId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public Question SelectById(int questionId)
        {
            const string sql = "SELECT QuestionID, QuizID, QuestionText, QuestionType, Marks FROM Questions WHERE QuestionID = @QuestionID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuestionID", questionId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public int Insert(Question q)
        {
            const string sql = @"
                INSERT INTO Questions (QuizID, QuestionText, QuestionType, Marks)
                VALUES (@QuizID, @QuestionText, @QuestionType, @Marks);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuizID", q.QuizID);
                DbHelper.AddParam(cmd, "@QuestionText", q.QuestionText);
                DbHelper.AddParam(cmd, "@QuestionType", q.QuestionType);
                DbHelper.AddParam(cmd, "@Marks", q.Marks);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Question q)
        {
            const string sql = @"
                UPDATE Questions SET QuestionText = @QuestionText, QuestionType = @QuestionType, Marks = @Marks
                WHERE QuestionID = @QuestionID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuestionText", q.QuestionText);
                DbHelper.AddParam(cmd, "@QuestionType", q.QuestionType);
                DbHelper.AddParam(cmd, "@Marks", q.Marks);
                DbHelper.AddParam(cmd, "@QuestionID", q.QuestionID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int questionId)
        {
            const string sql = "DELETE FROM Questions WHERE QuestionID = @QuestionID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuestionID", questionId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Question Map(SqlDataReader r)
        {
            return new Question
            {
                QuestionID   = DbHelper.GetInt(r, "QuestionID"),
                QuizID       = DbHelper.GetInt(r, "QuizID"),
                QuestionText = DbHelper.GetString(r, "QuestionText"),
                QuestionType = DbHelper.GetString(r, "QuestionType"),
                Marks        = DbHelper.GetInt(r, "Marks")
            };
        }
    }
}
