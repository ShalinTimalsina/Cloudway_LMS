using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    /// <summary>SQL for the Tags table and the CourseTags junction table together —
    /// they are small enough, and always used together, that one DAL class is clearer
    /// than two.</summary>
    public class TagDAL
    {
        public List<Tag> SelectAll()
        {
            List<Tag> list = new List<Tag>();
            const string sql = "SELECT TagID, TagName FROM Tags ORDER BY TagName;";

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

        public List<Tag> SelectByCourse(int courseId)
        {
            List<Tag> list = new List<Tag>();
            const string sql = @"
                SELECT t.TagID, t.TagName
                FROM   Tags t JOIN CourseTags ct ON t.TagID = ct.TagID
                WHERE  ct.CourseID = @CourseID
                ORDER BY t.TagName;";

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

        public int Insert(string tagName)
        {
            const string sql = @"
                INSERT INTO Tags (TagName) VALUES (@TagName);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@TagName", tagName);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Delete(int tagId)
        {
            const string sql = "DELETE FROM Tags WHERE TagID = @TagID;";
            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@TagID", tagId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>Links a tag to a course. Silently no-ops on a duplicate pair
        /// because CourseTags' primary key is (CourseID, TagID) — catch SqlException
        /// 2627 in the BLL if you want a friendly message instead.</summary>
        public void AddTagToCourse(int courseId, int tagId)
        {
            const string sql = "INSERT INTO CourseTags (CourseID, TagID) VALUES (@CourseID, @TagID);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                DbHelper.AddParam(cmd, "@TagID", tagId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveTagFromCourse(int courseId, int tagId)
        {
            const string sql = "DELETE FROM CourseTags WHERE CourseID = @CourseID AND TagID = @TagID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                DbHelper.AddParam(cmd, "@TagID", tagId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private Tag Map(SqlDataReader r)
        {
            return new Tag
            {
                TagID   = DbHelper.GetInt(r, "TagID"),
                TagName = DbHelper.GetString(r, "TagName")
            };
        }
    }
}
