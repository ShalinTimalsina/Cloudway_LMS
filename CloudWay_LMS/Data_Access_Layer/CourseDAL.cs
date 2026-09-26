using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class CourseDAL
    {
        public List<Course> SelectPublished(int? categoryId, int? tagId, string search)
        {
            List<Course> list = new List<Course>();

            const string sql = @"
                SELECT DISTINCT
                        c.CourseID, c.Title, c.Description, 
                        c.CategoryID, c.ThumbnailPath, c.IsPublished, 
                        c.CreatedBy, c.CreatedAt, cat.Name AS CategoryName
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                        LEFT JOIN  CourseTags ct  ON c.CourseID = ct.CourseID
                WHERE   c.IsPublished = 1
                  AND   (@CategoryID IS NULL OR c.CategoryID = @CategoryID)
                  AND   (@TagID IS NULL OR ct.TagID = @TagID)
                  AND   (@Search IS NULL OR c.Title LIKE '%' + @Search + '%'
                                         OR c.Description LIKE '%' + @Search + '%')
                ORDER BY c.CreatedAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryID", categoryId);
                DbHelper.AddParam(cmd, "@TagID", tagId);
                DbHelper.AddParam(cmd, "@Search", string.IsNullOrWhiteSpace(search) ? null : search.Trim());

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public List<Course> SelectPublishedPaged(
            int? categoryId, int? tagId, string search, int pageNumber, int pageSize, out int totalCount)
        {
            List<Course> pagedList = new List<Course>();

            const string countSql = @"
                SELECT COUNT(DISTINCT c.CourseID)
                FROM    Courses c
                        LEFT JOIN CourseTags ct ON c.CourseID = ct.CourseID
                WHERE   c.IsPublished = 1
                  AND   (@CategoryID IS NULL OR c.CategoryID = @CategoryID)
                  AND   (@TagID IS NULL OR ct.TagID = @TagID)
                  AND   (@Search IS NULL OR c.Title LIKE '%' + @Search + '%'
                                         OR c.Description LIKE '%' + @Search + '%');";

            const string pageSql = @"
                SELECT DISTINCT
                        c.CourseID, c.Title, c.Description,
                        c.CategoryID, c.ThumbnailPath, c.IsPublished,
                        c.CreatedBy, c.CreatedAt, cat.Name AS CategoryName
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                        LEFT JOIN  CourseTags ct  ON c.CourseID = ct.CourseID
                WHERE   c.IsPublished = 1
                  AND   (@CategoryID IS NULL OR c.CategoryID = @CategoryID)
                  AND   (@TagID IS NULL OR ct.TagID = @TagID)
                  AND   (@Search IS NULL OR c.Title LIKE '%' + @Search + '%'
                                         OR c.Description LIKE '%' + @Search + '%')
                ORDER BY c.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                string trimmedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

                using (SqlCommand countCmd = DbHelper.CreateCommand(con, countSql))
                {
                    DbHelper.AddParam(countCmd, "@CategoryID", categoryId);
                    DbHelper.AddParam(countCmd, "@TagID", tagId);
                    DbHelper.AddParam(countCmd, "@Search", trimmedSearch);
                    totalCount = (int)countCmd.ExecuteScalar();
                }

                using (SqlCommand cmd = DbHelper.CreateCommand(con, pageSql))
                {
                    DbHelper.AddParam(cmd, "@CategoryID", categoryId);
                    DbHelper.AddParam(cmd, "@TagID", tagId);
                    DbHelper.AddParam(cmd, "@Search", trimmedSearch);
                    DbHelper.AddParam(cmd, "@Offset", (pageNumber - 1) * pageSize);
                    DbHelper.AddParam(cmd, "@PageSize", pageSize);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read()) pagedList.Add(Map(r));
                    }
                }
            }

            return pagedList;
        }

        public List<Course> SelectFeatured(int count)
        {
            List<Course> list = new List<Course>();
            const string sql = @"
                SELECT TOP (@Count)
                        c.CourseID, c.Title, c.Description, c.ThumbnailPath,
                        c.CategoryID, c.IsPublished, c.CreatedBy, c.CreatedAt,
                        cat.Name AS CategoryName
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                WHERE   c.IsPublished = 1
                ORDER BY c.CreatedAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@Count", count);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public List<Course> SelectAll()
        {
            List<Course> list = new List<Course>();
            const string sql = @"
                SELECT  c.CourseID, c.Title, c.Description,
                        c.CategoryID, c.ThumbnailPath, c.IsPublished,
                        c.CreatedBy, c.CreatedAt, cat.Name AS CategoryName,
                        u.FullName AS CreatedByName,
                        (SELECT COUNT(*) FROM Enrollments e WHERE e.CourseID = c.CourseID) AS EnrollmentCount
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                        INNER JOIN Users      u   ON c.CreatedBy  = u.UserID
                ORDER BY c.CreatedAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Course c = Map(r);
                        c.CreatedByName = DbHelper.GetString(r, "CreatedByName");
                        c.EnrollmentCount = DbHelper.GetInt(r, "EnrollmentCount");
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        public Course SelectById(int courseId)
        {
            const string sql = @"
                SELECT  c.CourseID, c.Title, c.Description,
                        c.CategoryID, c.ThumbnailPath, c.IsPublished,
                        c.CreatedBy, c.CreatedAt, cat.Name AS CategoryName
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                WHERE   c.CourseID = @CourseID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        public int Insert(Course c)
        {
            const string sql = @"
                INSERT INTO Courses
                    (Title, Description, CategoryID, ThumbnailPath, IsPublished, CreatedBy)
                VALUES
                    (@Title, @Description, @CategoryID, @ThumbnailPath, @IsPublished, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                AddCourseParams(cmd, c);
                DbHelper.AddParam(cmd, "@CreatedBy", c.CreatedBy);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Course c)
        {
            const string sql = @"
                UPDATE Courses
                SET Title = @Title, Description = @Description,
                    CategoryID = @CategoryID, ThumbnailPath = @ThumbnailPath,
                    IsPublished = @IsPublished
                WHERE CourseID = @CourseID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                AddCourseParams(cmd, c);
                DbHelper.AddParam(cmd, "@CourseID", c.CourseID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int courseId)
        {
            const string sql = "DELETE FROM Courses WHERE CourseID = @CourseID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CourseID", courseId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private void AddCourseParams(SqlCommand cmd, Course c)
        {
            DbHelper.AddParam(cmd, "@Title", c.Title);
            DbHelper.AddParam(cmd, "@Description", c.Description);
            DbHelper.AddParam(cmd, "@CategoryID", c.CategoryID);
            DbHelper.AddParam(cmd, "@ThumbnailPath", c.ThumbnailPath);
            DbHelper.AddParam(cmd, "@IsPublished", c.IsPublished);
        }

        private Course Map(SqlDataReader r)
        {
            return new Course
            {
                CourseID = DbHelper.GetInt(r, "CourseID"),
                Title = DbHelper.GetString(r, "Title"),
                Description = HasColumn(r, "Description") ? DbHelper.GetString(r, "Description") : null,
                CategoryID = DbHelper.GetInt(r, "CategoryID"),
                ThumbnailPath = DbHelper.GetString(r, "ThumbnailPath"),
                IsPublished = DbHelper.GetBool(r, "IsPublished"),
                CreatedBy = DbHelper.GetInt(r, "CreatedBy"),
                CreatedAt = DbHelper.GetDate(r, "CreatedAt"),
                CategoryName = DbHelper.GetString(r, "CategoryName")
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
