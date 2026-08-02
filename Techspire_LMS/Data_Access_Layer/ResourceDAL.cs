using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    /// <summary>SQL for the generic per-lesson attachment table (code files, PCAPs,
    /// slide decks, cheat sheets — anything downloadable). Store paths, not files.</summary>
    public class ResourceDAL
    {
        public List<Resource> SelectByLesson(int lessonId)
        {
            List<Resource> list = new List<Resource>();
            const string sql = @"
                SELECT ResourceID, LessonID, Title, FilePath, ResourceType, UploadedAt
                FROM   Resources
                WHERE  LessonID = @LessonID
                ORDER BY UploadedAt;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@LessonID", lessonId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public int Insert(Resource res)
        {
            const string sql = @"
                INSERT INTO Resources (LessonID, Title, FilePath, ResourceType, UploadedAt)
                VALUES (@LessonID, @Title, @FilePath, @ResourceType, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@LessonID", res.LessonID);
                DbHelper.AddParam(cmd, "@Title", res.Title);
                DbHelper.AddParam(cmd, "@FilePath", res.FilePath);
                DbHelper.AddParam(cmd, "@ResourceType", res.ResourceType);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Delete(int resourceId)
        {
            const string sql = "DELETE FROM Resources WHERE ResourceID = @ResourceID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@ResourceID", resourceId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Resource Map(SqlDataReader r)
        {
            return new Resource
            {
                ResourceID   = DbHelper.GetInt(r, "ResourceID"),
                LessonID     = DbHelper.GetInt(r, "LessonID"),
                Title        = DbHelper.GetString(r, "Title"),
                FilePath     = DbHelper.GetString(r, "FilePath"),
                ResourceType = DbHelper.GetString(r, "ResourceType"),
                UploadedAt   = DbHelper.GetDate(r, "UploadedAt")
            };
        }
    }
}
