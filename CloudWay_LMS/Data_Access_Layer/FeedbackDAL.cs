using System.Collections.Generic;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class FeedbackDAL
    {
        public List<Feedback> SelectAll()
        {
            List<Feedback> list = new List<Feedback>();
            const string sql = @"
                SELECT FeedbackID, UserID, Name, Email, Subject, Message, IsRead, SubmittedAt
                FROM   Feedback
                ORDER BY SubmittedAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        /// <summary>UserID is nullable — NULL means a guest (not logged in) submitted the form.</summary>
        public int Insert(Feedback f)
        {
            const string sql = @"
                INSERT INTO Feedback (UserID, Name, Email, Subject, Message, IsRead, SubmittedAt)
                VALUES (@UserID, @Name, @Email, @Subject, @Message, 0, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", f.UserID);
                DbHelper.AddParam(cmd, "@Name", f.Name);
                DbHelper.AddParam(cmd, "@Email", f.Email);
                DbHelper.AddParam(cmd, "@Subject", f.Subject);
                DbHelper.AddParam(cmd, "@Message", f.Message);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool MarkRead(int feedbackId)
        {
            const string sql = "UPDATE Feedback SET IsRead = 1 WHERE FeedbackID = @FeedbackID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@FeedbackID", feedbackId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int feedbackId)
        {
            const string sql = "DELETE FROM Feedback WHERE FeedbackID = @FeedbackID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@FeedbackID", feedbackId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Feedback Map(SqlDataReader r)
        {
            return new Feedback
            {
                FeedbackID  = DbHelper.GetInt(r, "FeedbackID"),
                UserID      = DbHelper.GetNullableInt(r, "UserID"),
                Name        = DbHelper.GetString(r, "Name"),
                Email       = DbHelper.GetString(r, "Email"),
                Subject     = DbHelper.GetString(r, "Subject"),
                Message     = DbHelper.GetString(r, "Message"),
                IsRead      = DbHelper.GetBool(r, "IsRead"),
                SubmittedAt = DbHelper.GetDate(r, "SubmittedAt")
            };
        }
    }
}
