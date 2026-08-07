using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Techspire_LMS.Models;

namespace Techspire_LMS.Data_Access_Layer
{
    /// <summary>
    /// All SQL for the Courses table. Every method follows the same five-step
    /// shape: 1. open connection  2. build command  3. add parameters
    ///        4. execute          5. map results to models
    /// </summary>
    public class CourseDAL
    {
        // ==================================================================
        // READ — public catalogue: published only, optional category/tag/search filters
        // ==================================================================
        public List<Course> SelectPublished(int? categoryId, int? tagId, string search)
        {
            List<Course> list = new List<Course>();

            // (@Param IS NULL OR column = @Param) skips a filter with no
            // string concatenation, so there is no injection surface here.
            const string sql = @"
                SELECT DISTINCT
                        c.CourseID, c.Title, c.ShortDescription, c.FullDescription,
                        c.CategoryID, c.ThumbnailPath, c.DifficultyLevel,
                        c.DurationMinutes, c.IsPublished, c.CreatedBy, c.CreatedAt, c.UpdatedAt,
                        cat.CategoryName
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                        LEFT JOIN  CourseTags ct  ON c.CourseID = ct.CourseID
                WHERE   c.IsPublished = 1
                  AND   (@CategoryID IS NULL OR c.CategoryID = @CategoryID)
                  AND   (@TagID IS NULL OR ct.TagID = @TagID)
                  AND   (@Search IS NULL OR c.Title LIKE '%' + @Search + '%'
                                         OR c.ShortDescription LIKE '%' + @Search + '%')
                ORDER BY c.CreatedAt DESC;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@CategoryID", categoryId);
                DbHelper.AddParam(cmd, "@TagID", tagId);
                DbHelper.AddParam(cmd, "@Search",
                    string.IsNullOrWhiteSpace(search) ? null : search.Trim());

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        // ==================================================================
        // READ — same filters as SelectPublished, but SQL-level paged via
        // OFFSET/FETCH instead of returning the whole result set. Used by the
        // public catalogue, which could realistically grow to hundreds of
        // rows — unlike the admin grids (see ManageCourses.aspx's in-memory
        // GridView paging), pulling every row on every page load here would
        // get slower as the catalogue grows, not just look worse.
        // ==================================================================
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
                                         OR c.ShortDescription LIKE '%' + @Search + '%');";

            const string pageSql = @"
                SELECT DISTINCT
                        c.CourseID, c.Title, c.ShortDescription, c.FullDescription,
                        c.CategoryID, c.ThumbnailPath, c.DifficultyLevel,
                        c.DurationMinutes, c.IsPublished, c.CreatedBy, c.CreatedAt, c.UpdatedAt,
                        cat.CategoryName
                FROM    Courses c
                        INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
                        LEFT JOIN  CourseTags ct  ON c.CourseID = ct.CourseID
                WHERE   c.IsPublished = 1
                  AND   (@CategoryID IS NULL OR c.CategoryID = @CategoryID)
                  AND   (@TagID IS NULL OR ct.TagID = @TagID)
                  AND   (@Search IS NULL OR c.Title LIKE '%' + @Search + '%'
                                         OR c.ShortDescription LIKE '%' + @Search + '%')
                ORDER BY c.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            // ORDER BY is mandatory before OFFSET/FETCH in SQL Server — a
            // paged query needs a stable sort or you can get the same row
            // appearing on two different pages if the query plan shifts
            // between requests. CreatedAt works here because it's set once
            // at insert and never changes.

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

        // ==================================================================
        public List<Course> SelectFeatured(int count)
        {
            List<Course> list = new List<Course>();
            const string sql = @"
                SELECT TOP (@Count)
                        c.CourseID, c.Title, c.ShortDescription, c.ThumbnailPath,
                        c.DifficultyLevel, c.DurationMinutes, c.CategoryID,
                        c.CreatedAt, c.UpdatedAt, c.IsPublished, c.CreatedBy,
                        cat.CategoryName
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

        // ==================================================================
        // READ — everything, including drafts (admin grid)
        // ==================================================================
        public List<Course> SelectAll()
        {
            List<Course> list = new List<Course>();
            const string sql = @"
                SELECT  c.CourseID, c.Title, c.ShortDescription, c.FullDescription,
                        c.CategoryID, c.ThumbnailPath, c.DifficultyLevel,
                        c.DurationMinutes, c.IsPublished, c.CreatedBy, c.CreatedAt, c.UpdatedAt,
                        cat.CategoryName,
                        u.FullName AS CreatedByName,
                        (SELECT COUNT(*) FROM Enrollments e WHERE e.CourseID = c.CourseID) AS EnrolmentCount
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
                        c.EnrolmentCount = DbHelper.GetInt(r, "EnrolmentCount");
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        // ==================================================================
        // READ — one course by ID. Returns null when not found.
        // ==================================================================
        public Course SelectById(int courseId)
        {
            const string sql = @"
                SELECT  c.CourseID, c.Title, c.ShortDescription, c.FullDescription,
                        c.CategoryID, c.ThumbnailPath, c.DifficultyLevel,
                        c.DurationMinutes, c.IsPublished, c.CreatedBy, c.CreatedAt, c.UpdatedAt,
                        cat.CategoryName
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

        // ==================================================================
        // CREATE
        // ==================================================================
        public int Insert(Course c)
        {
            const string sql = @"
                INSERT INTO Courses
                    (Title, ShortDescription, FullDescription, CategoryID, ThumbnailPath,
                     DifficultyLevel, DurationMinutes, IsPublished, CreatedBy, CreatedAt, UpdatedAt)
                VALUES
                    (@Title, @ShortDescription, @FullDescription, @CategoryID, @ThumbnailPath,
                     @DifficultyLevel, @DurationMinutes, @IsPublished, @CreatedBy, SYSUTCDATETIME(), SYSUTCDATETIME());
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

        // ==================================================================
        // UPDATE
        // ==================================================================
        public bool Update(Course c)
        {
            const string sql = @"
                UPDATE Courses
                SET Title = @Title, ShortDescription = @ShortDescription, FullDescription = @FullDescription,
                    CategoryID = @CategoryID, ThumbnailPath = @ThumbnailPath, DifficultyLevel = @DifficultyLevel,
                    DurationMinutes = @DurationMinutes, IsPublished = @IsPublished, UpdatedAt = SYSUTCDATETIME()
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

        // ==================================================================
        // DELETE — cascades to Lessons/Resources/Quizzes/Questions/QuestionOptions
        // via ON DELETE CASCADE, but NOT to Enrollments (kept intentionally so
        // enrolment history isn't silently destroyed — see design doc).
        // ==================================================================
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
            DbHelper.AddParam(cmd, "@ShortDescription", c.ShortDescription);
            DbHelper.AddParam(cmd, "@FullDescription", c.FullDescription);
            DbHelper.AddParam(cmd, "@CategoryID", c.CategoryID);
            DbHelper.AddParam(cmd, "@ThumbnailPath", c.ThumbnailPath);
            DbHelper.AddParam(cmd, "@DifficultyLevel", c.DifficultyLevel);
            DbHelper.AddParam(cmd, "@DurationMinutes", c.DurationMinutes);
            DbHelper.AddParam(cmd, "@IsPublished", c.IsPublished);
        }

        private Course Map(SqlDataReader r)
        {
            return new Course
            {
                CourseID = DbHelper.GetInt(r, "CourseID"),
                Title = DbHelper.GetString(r, "Title"),
                ShortDescription = DbHelper.GetString(r, "ShortDescription"),
                FullDescription = HasColumn(r, "FullDescription") ? DbHelper.GetString(r, "FullDescription") : null,
                CategoryID = DbHelper.GetInt(r, "CategoryID"),
                ThumbnailPath = DbHelper.GetString(r, "ThumbnailPath"),
                DifficultyLevel = DbHelper.GetString(r, "DifficultyLevel"),
                DurationMinutes = DbHelper.GetNullableInt(r, "DurationMinutes"),
                IsPublished = DbHelper.GetBool(r, "IsPublished"),
                CreatedBy = DbHelper.GetInt(r, "CreatedBy"),
                CreatedAt = DbHelper.GetDate(r, "CreatedAt"),
                UpdatedAt = DbHelper.GetDate(r, "UpdatedAt"),
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
