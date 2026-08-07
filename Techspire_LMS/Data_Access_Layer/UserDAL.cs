using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Techspire_LMS.Models;


namespace Techspire_LMS.Data_Access_Layer
{

    /// <summary>All SQL for the Users table. No hashing, no session logic — that's the BLL.</summary>
    public class UserDAL
    {
        // Shared column list — every SELECT below needs the same columns, and
        // forgetting FailedLoginAttempts/LockoutEndUtc on just one of them
        // would silently break AuthBLL.Login's lockout check for whichever
        // path used it. One constant, three call sites.
        private const string SelectColumns = @"
                u.UserID, u.FullName, u.Email, u.PasswordHash, u.PasswordSalt,
                u.RoleID, u.PhoneNumber, u.IsActive, u.CreatedAt, u.LastLoginAt,
                u.FailedLoginAttempts, u.LockoutEndUtc,
                ro.RoleName";

        public List<User> SelectAll()
        {
            List<User> list = new List<User>();
            string sql =
                "SELECT " + SelectColumns + @"
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
            string sql =
                "SELECT " + SelectColumns + @"
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
            string sql =
                "SELECT " + SelectColumns + @"
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

        /// <summary>Insert and return the new UserID. RoleID defaults to the "Member" row (see seed data).
        /// FailedLoginAttempts/LockoutEndUtc need no explicit values — the
        /// column defaults (0 / NULL) cover a brand-new account.</summary>
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

        public bool Reactivate(int userId)
        {
            const string sql = "UPDATE Users SET IsActive = 1 WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>Promote/demote between Roles — e.g. Member (2) to Admin (1).</summary>
        public bool ChangeRole(int userId, int roleId)
        {
            const string sql = "UPDATE Users SET RoleID = @RoleID WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@RoleID", roleId);
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>Called after a failed password check. newFailedCount is
        /// whatever AuthBLL decided it should be (it owns the threshold
        /// logic); lockoutEndUtc is null unless this failure just crossed
        /// the threshold. Two responsibilities, one UPDATE, because they
        /// always change together.</summary>
        public void RecordFailedLogin(int userId, int newFailedCount, System.DateTime? lockoutEndUtc)
        {
            const string sql = @"
                UPDATE Users SET FailedLoginAttempts = @FailedLoginAttempts, LockoutEndUtc = @LockoutEndUtc
                WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@FailedLoginAttempts", newFailedCount);
                DbHelper.AddParam(cmd, "@LockoutEndUtc", lockoutEndUtc);
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Called on a SUCCESSFUL login — clears the counter and any
        /// lockout so the next mistake starts counting from zero again.</summary>
        public void ResetFailedLogins(int userId)
        {
            const string sql = "UPDATE Users SET FailedLoginAttempts = 0, LockoutEndUtc = NULL WHERE UserID = @UserID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@UserID", userId);
                con.Open();
                cmd.ExecuteNonQuery();
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
                FailedLoginAttempts = DbHelper.GetInt(r, "FailedLoginAttempts"),
                LockoutEndUtc = DbHelper.GetNullableDate(r, "LockoutEndUtc"),
                RoleName = DbHelper.GetString(r, "RoleName")
            };
        }
    }

    }