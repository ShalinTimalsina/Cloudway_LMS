using System.Collections.Generic;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class QuizAttemptDAL
    {
        public List<QuizAttempt> SelectByUser(int userId)
        {
            List<QuizAttempt> list = new List<QuizAttempt>();
            const string sql = @"
                SELECT a.AttemptID, a.UserID, a.QuizID, a.Score, a.TotalMarks, a.IsPassed, a.AttemptedAt,
                       q.Title AS QuizTitle, q.PassingScore
                FROM   QuizAttempts a JOIN Quizzes q ON a.QuizID = q.QuizID
                WHERE  a.UserID = @UserID
                ORDER BY a.AttemptedAt DESC;";

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

        public List<QuizAttempt> SelectByQuiz(int quizId)
        {
            List<QuizAttempt> list = new List<QuizAttempt>();
            const string sql = @"
                SELECT AttemptID, UserID, QuizID, Score, TotalMarks, IsPassed, AttemptedAt
                FROM   QuizAttempts
                WHERE  QuizID = @QuizID
                ORDER BY AttemptedAt DESC;";

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

        /// <summary>Score/TotalMarks/IsPassed are computed by the BLL against the
        /// question set and PassMark AT THE TIME OF THE ATTEMPT, then stored here
        /// as a snapshot — so a later edit to the quiz never changes a past result.</summary>
        public int Insert(QuizAttempt a)
        {
            const string sql = @"
                INSERT INTO QuizAttempts (UserID, QuizID, Score, TotalMarks, IsPassed, AttemptedAt)
                VALUES (@UserID, @QuizID, @Score, @TotalMarks, @IsPassed, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", a.UserID);
                DbHelper.AddParam(cmd, "@QuizID", a.QuizID);
                DbHelper.AddParam(cmd, "@Score", a.Score);
                DbHelper.AddParam(cmd, "@TotalMarks", a.TotalMarks);
                DbHelper.AddParam(cmd, "@IsPassed", a.IsPassed);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        private QuizAttempt Map(SqlDataReader r)
        {
            var a = new QuizAttempt
            {
                AttemptID   = DbHelper.GetInt(r, "AttemptID"),
                UserID      = DbHelper.GetInt(r, "UserID"),
                QuizID      = DbHelper.GetInt(r, "QuizID"),
                Score       = DbHelper.GetInt(r, "Score"),
                TotalMarks  = DbHelper.GetInt(r, "TotalMarks"),
                IsPassed    = DbHelper.GetBool(r, "IsPassed"),
                AttemptedAt = DbHelper.GetDate(r, "AttemptedAt")
            };
            if (HasColumn(r, "QuizTitle")) a.QuizTitle = DbHelper.GetString(r, "QuizTitle");
            return a;
        }

        private bool HasColumn(System.Data.IDataRecord r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (r.GetName(i).Equals(col)) return true;
            return false;
        }
    }
}
