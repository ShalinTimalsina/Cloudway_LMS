
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    public class CategoryDAL
    {
        public List<Category> SelectAll()
        {
            List<Category> list = new List<Category>();
            const string sql = @"
                SELECT c.CategoryID, c.CategoryName, c.Description, c.IsActive,
                       (SELECT COUNT(*) FROM Courses co WHERE co.CategoryID = c.CategoryID) AS CourseCount
                FROM   Categories c
                ORDER BY c.CategoryName;";

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

        public List<Category> SelectActive()
        {
            List<Category> list = new List<Category>();
            const string sql = @"
                SELECT CategoryID, CategoryName, Description, IsActive
                FROM   Categories
                WHERE  IsActive = 1
                ORDER BY CategoryName;";

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

        public Category SelectById(int categoryId)
        {
            const string sql = "SELECT CategoryID, CategoryName, Description, IsActive FROM Categories WHERE CategoryID = @CategoryID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryID", categoryId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public int Insert(Category c)
        {
            const string sql = @"
                INSERT INTO Categories (CategoryName, Description, IsActive)
                VALUES (@CategoryName, @Description, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryName", c.CategoryName);
                DbHelper.AddParam(cmd, "@Description", c.Description);
                DbHelper.AddParam(cmd, "@IsActive", c.IsActive);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Category c)
        {
            const string sql = @"
                UPDATE Categories
                SET CategoryName = @CategoryName, Description = @Description, IsActive = @IsActive
                WHERE CategoryID = @CategoryID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryName", c.CategoryName);
                DbHelper.AddParam(cmd, "@Description", c.Description);
                DbHelper.AddParam(cmd, "@IsActive", c.IsActive);
                DbHelper.AddParam(cmd, "@CategoryID", c.CategoryID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>Hard delete is safe here only because Courses.CategoryID has no CASCADE —
        /// SQL Server will reject the delete (error 547) while courses still reference it.
        /// Catch that in the BLL and show "This category still has courses" instead of a raw error.</summary>
        public bool Delete(int categoryId)
        {
            const string sql = "DELETE FROM Categories WHERE CategoryID = @CategoryID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryID", categoryId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Category Map(SqlDataReader r)
        {
            return new Category
            {
                CategoryID   = DbHelper.GetInt(r, "CategoryID"),
                CategoryName = DbHelper.GetString(r, "CategoryName"),
                Description  = DbHelper.GetString(r, "Description"),
                IsActive     = DbHelper.GetBool(r, "IsActive"),
                CourseCount  = HasColumn(r, "CourseCount") ? DbHelper.GetInt(r, "CourseCount") : 0
            };
        }

        private bool HasColumn(IDataRecord r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (r.GetName(i).Equals(col)) return true;
            return false;
        }
    }
}
