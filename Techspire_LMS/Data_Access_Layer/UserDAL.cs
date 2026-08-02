using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Techspire_LMS.Models;


namespace Techspire_LMS.Data_Access_Layer
{

    /// <summary>All SQL for the Users table. No hashing, no session logic — that's the BLL.</summary>
    public class UserDAL
    {
        public List<User> SelectAll()
        {
            List<User> list = new List<User>();
            const string sql = @"
                SELECT u.UserID, u.FullName, u.Email, u.PasswordHash, u.PasswordSalt,
                       u.RoleID, u.PhoneNumber, u.IsActive, u.CreatedAt, u.LastLoginAt,
                       ro.RoleName
                FROM   Users u JOIN Roles ro ON u.RoleID = ro.RoleID
                ORDER BY u.FullName;";

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

        public User SelectById(int userId)
        {
            const string sql = @"
                SELECT u.UserID, u.FullName, u.Email, u.PasswordHash, u.PasswordSalt,
                       u.RoleID, u.PhoneNumber, u.IsActive, u.CreatedAt, u.LastLoginAt,
                       ro.RoleName
                FROM   Users u JOIN Roles ro ON u.RoleID = ro.RoleID
                WHERE  u.UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        /// <summary>Used by login. Returns null if no account has this email.</summary>
        public User SelectByEmail(string email)
        {
            const string sql = @"
                SELECT u.UserID, u.FullName, u.Email, u.PasswordHash, u.PasswordSalt,
                       u.RoleID, u.PhoneNumber, u.IsActive, u.CreatedAt, u.LastLoginAt,
                       ro.RoleName
                FROM   Users u JOIN Roles ro ON u.RoleID = ro.RoleID
                WHERE  u.Email = @Email;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@Email", email);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        /// <summary>Insert and return the new UserID. RoleID defaults to the "Member" row (see seed data).</summary>
        public int Insert(User u)
        {
            const string sql = @"
                INSERT INTO Users (FullName, Email, PasswordHash, PasswordSalt, RoleID, PhoneNumber, IsActive, CreatedAt)
                VALUES (@FullName, @Email, @PasswordHash, @PasswordSalt, @RoleID, @PhoneNumber, 1, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@FullName", u.FullName);
                DbHelper.AddParam(cmd, "@Email", u.Email);
                DbHelper.AddParam(cmd, "@PasswordHash", u.PasswordHash);
                DbHelper.AddParam(cmd, "@PasswordSalt", u.PasswordSalt);
                DbHelper.AddParam(cmd, "@RoleID", u.RoleID);
                DbHelper.AddParam(cmd, "@PhoneNumber", u.PhoneNumber);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Updates profile fields only — never touches the password. See ChangePassword.</summary>
        public bool Update(User u)
        {
            const string sql = @"
                UPDATE Users
                SET FullName = @FullName, PhoneNumber = @PhoneNumber, IsActive = @IsActive
                WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@FullName", u.FullName);
                DbHelper.AddParam(cmd, "@PhoneNumber", u.PhoneNumber);
                DbHelper.AddParam(cmd, "@IsActive", u.IsActive);
                DbHelper.AddParam(cmd, "@UserID", u.UserID);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ChangePassword(int userId, string newHash, string newSalt)
        {
            const string sql = @"
                UPDATE Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt
                WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@PasswordHash", newHash);
                DbHelper.AddParam(cmd, "@PasswordSalt", newSalt);
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public void UpdateLastLogin(int userId)
        {
            const string sql = "UPDATE Users SET LastLoginAt = SYSUTCDATETIME() WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Soft delete — keeps enrolment/attempt history intact. Prefer this over a hard DELETE.</summary>
        public bool Deactivate(int userId)
        {
            const string sql = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private User Map(SqlDataReader r)
        {
            return new User
            {
                UserID = DbHelper.GetInt(r, "UserID"),
                FullName = DbHelper.GetString(r, "FullName"),
                Email = DbHelper.GetString(r, "Email"),
                PasswordHash = DbHelper.GetString(r, "PasswordHash"),
                PasswordSalt = DbHelper.GetString(r, "PasswordSalt"),
                RoleID = DbHelper.GetInt(r, "RoleID"),
                PhoneNumber = DbHelper.GetString(r, "PhoneNumber"),
                IsActive = DbHelper.GetBool(r, "IsActive"),
                CreatedAt = DbHelper.GetDate(r, "CreatedAt"),
                LastLoginAt = DbHelper.GetNullableDate(r, "LastLoginAt"),
                RoleName = DbHelper.GetString(r, "RoleName")
            };
        }
    }

}