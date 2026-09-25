using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class CategoryDAL
    {
        public List<Category> SelectAll()
        {
            List<Category> list = new List<Category>();
            const string sql = @"
                SELECT c.CategoryID, c.Name, c.Description,
                       (SELECT COUNT(*) FROM Courses co WHERE co.CategoryID = c.CategoryID) AS CourseCount
                FROM   Categories c
                ORDER BY c.Name;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Category c = Map(r);
                        c.CourseCount = DbHelper.GetInt(r, "CourseCount");
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        public List<Category> SelectActive()
        {
            List<Category> list = new List<Category>();
            const string sql = @"
                SELECT CategoryID, Name, Description
                FROM   Categories
                ORDER BY Name;";

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
            const string sql = "SELECT CategoryID, Name, Description FROM Categories WHERE CategoryID = @CategoryID;";

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

        public int Insert(Category cat)
        {
            const string sql = @"
                INSERT INTO Categories (Name, Description)
                VALUES (@Name, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@Name", cat.Name);
                DbHelper.AddParam(cmd, "@Description", cat.Description);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Category cat)
        {
            const string sql = @"
                UPDATE Categories
                SET    Name = @Name, Description = @Description
                WHERE  CategoryID = @CategoryID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryID", cat.CategoryID);
                DbHelper.AddParam(cmd, "@Name", cat.Name);
                DbHelper.AddParam(cmd, "@Description", cat.Description);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

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
                CategoryID = DbHelper.GetInt(r, "CategoryID"),
                Name = DbHelper.GetString(r, "Name"),
                Description = HasColumn(r, "Description") ? DbHelper.GetString(r, "Description") : null
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
