using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    /// <summary>Fully normalised options for a Question — any number of rows,
    /// any number flagged IsCorrect, unlike a fixed OptionA..OptionD design.
    /// This is what lets one schema support single-answer, multi-answer and
    /// true/false quizzes without a redesign.</summary>
    public class QuestionOptionDAL
    {
        public List<QuestionOption> SelectByQuestion(int questionId)
        {
            List<QuestionOption> list = new List<QuestionOption>();
            const string sql = @"
                SELECT OptionID, QuestionID, OptionText, IsCorrect, SortOrder
                FROM   QuestionOptions
                WHERE  QuestionID = @QuestionID
                ORDER BY SortOrder;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuestionID", questionId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public int Insert(QuestionOption o)
        {
            const string sql = @"
                INSERT INTO QuestionOptions (QuestionID, OptionText, IsCorrect, SortOrder)
                VALUES (@QuestionID, @OptionText, @IsCorrect, @SortOrder);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@QuestionID", o.QuestionID);
                DbHelper.AddParam(cmd, "@OptionText", o.OptionText);
                DbHelper.AddParam(cmd, "@IsCorrect", o.IsCorrect);
                DbHelper.AddParam(cmd, "@SortOrder", o.SortOrder);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(QuestionOption o)
        {
            const string sql = @"
                UPDATE QuestionOptions SET OptionText = @OptionText, IsCorrect = @IsCorrect, SortOrder = @SortOrder
                WHERE OptionID = @OptionID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@OptionText", o.OptionText);
                DbHelper.AddParam(cmd, "@IsCorrect", o.IsCorrect);
                DbHelper.AddParam(cmd, "@SortOrder", o.SortOrder);
                DbHelper.AddParam(cmd, "@OptionID", o.OptionID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int optionId)
        {
            const string sql = "DELETE FROM QuestionOptions WHERE OptionID = @OptionID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@OptionID", optionId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private QuestionOption Map(SqlDataReader r)
        {
            return new QuestionOption
            {
                OptionID   = DbHelper.GetInt(r, "OptionID"),
                QuestionID = DbHelper.GetInt(r, "QuestionID"),
                OptionText = DbHelper.GetString(r, "OptionText"),
                IsCorrect  = DbHelper.GetBool(r, "IsCorrect"),
                SortOrder  = DbHelper.GetInt(r, "SortOrder")
            };
        }
    }
}
