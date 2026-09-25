// ============================================================================
// CreateAdmin.cs — one-off command-line tool to create (or promote) an
// Admin account. Solves a real chicken-and-egg problem: Account/Register.aspx
// always creates a Member (AuthBLL.Register hard-codes that — correctly,
// self-registration should never hand out Admin), and the promote-to-Admin
// button on Admin/ManageUsers.aspx is itself behind an Admin-only page. With
// zero admins in the database, nobody can reach the page that creates the
// first one. This file breaks that cycle from outside the web app.
//
// HOW TO RUN (no project, no Visual Studio "New Project" needed):
//
//   1. Save this file at the root of your CloudWay_LMS project, next to
//      the Helpers/ folder (so the relative path below resolves).
//   2. Edit CONNECTION_STRING below to match your SQL Server instance —
//      copy it straight from Web.config if you've already customised it.
//   3. Open a terminal (VS's View > Terminal, or plain cmd/PowerShell) in
//      that same folder and run:
//
//        & "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
//          /nologo /out:CreateAdmin.exe CreateAdmin.cs Helpers\PasswordHelper.cs `
//          /reference:System.Data.dll
//
//      (PowerShell syntax above — for cmd.exe, use one line instead:
//
//        %WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /out:CreateAdmin.exe CreateAdmin.cs Helpers\PasswordHelper.cs /reference:System.Data.dll
//
//      If your install is 32-bit .NET Framework, use Framework instead of
//      Framework64 in either path.)
//
//   4. Run the result:
//
//        .\CreateAdmin.exe
//
// WHY THIS STILL MATTERS: this tool passes Helpers\PasswordHelper.cs to the
// compiler as a SECOND source file — not a copy, the actual file — so the
// PasswordHelper class compiled into CreateAdmin.exe is byte-for-byte the
// same code Account/Login.aspx verifies against. Whatever hash this tool
// writes to the database, a real login with that exact password WILL
// verify successfully. Everything else here (the SQL) is a small,
// deliberate, one-off duplication of what UserDAL already does — not
// reused, because UserDAL's DbHelper depends on ConfigurationManager
// reading a Web.config a bare compiled .exe doesn't have; that dependency
// isn't worth chasing for a tool you run once or twice per project.
// ============================================================================

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using CloudWay_LMS.Helpers; // PasswordHelper — see the top-of-file comment



namespace CloudWay_LMS.Tools
{
    internal class CreateAdmin
    {
        // ---- EDIT THIS to match your Web.config's connection string ----
        private const string ConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LearningPlatformDB;Integrated Security=True;TrustServerCertificate=True";

        private const int MinPasswordLength = 8; // mirrors AuthBLL.Register's rule
        private static readonly Regex EmailPattern =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static void Main()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine(" CloudWay_LMS — Create / Promote an Admin User");
            Console.WriteLine("=================================================");
            Console.WriteLine();

            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("FAILED: " + ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void Run()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                int adminRoleId = FindAdminRoleId(con);
                string email = PromptEmail();

                int existingUserId;
                string existingName, existingRoleName;
                bool exists = TryFindUserByEmail(con, email, out existingUserId, out existingName, out existingRoleName);

                if (exists)
                {
                    HandleExistingUser(con, existingUserId, existingName, existingRoleName, adminRoleId, email);
                }
                else
                {
                    CreateNewAdmin(con, email, adminRoleId);
                }
            }
        }

        /// <summary>Looks the Admin role up BY NAME rather than assuming
        /// RoleID = 1 — CreateDatabase.sql's seed data happens to insert
        /// Admin first, but asking the database "which row is actually
        /// named Admin" costs one query and removes that assumption
        /// entirely. Same principle CourseBLL/CategoryBLL follow everywhere
        /// else: look things up, don't assume IDs.</summary>
        private static int FindAdminRoleId(SqlConnection con)
        {
            const string sql = "SELECT RoleID FROM Roles WHERE RoleName = 'Admin';";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                object result = cmd.ExecuteScalar();
                if (result == null)
                    throw new InvalidOperationException(
                        "No role named 'Admin' exists in the Roles table. Run CreateDatabase.sql first.");
                return (int)result;
            }
        }

        private static string PromptEmail()
        {
            while (true)
            {
                Console.Write("Email: ");
                string email = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (EmailPattern.IsMatch(email)) return email;
                Console.WriteLine("  That doesn't look like a valid email address. Try again.");
            }
        }

        private static bool TryFindUserByEmail(
            SqlConnection con, string email, out int userId, out string fullName, out string roleName)
        {
            const string sql = @"
                SELECT u.UserID, u.FullName, ro.RoleName
                FROM   Users u JOIN Roles ro ON u.RoleID = ro.RoleID
                WHERE  u.Email = @Email;";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        userId = 0; fullName = null; roleName = null;
                        return false;
                    }
                    userId = (int)r["UserID"];
                    fullName = (string)r["FullName"];
                    roleName = (string)r["RoleName"];
                    return true;
                }
            }
        }

        private static void HandleExistingUser(
            SqlConnection con, int userId, string fullName, string roleName, int adminRoleId, string email)
        {
            Console.WriteLine();
            Console.WriteLine("A user with that email already exists: " + fullName + " (currently " + roleName + ").");

            if (roleName == "Admin")
            {
                Console.WriteLine("They're already an Admin — nothing to do.");
                return;
            }

            Console.Write("Promote this existing account to Admin instead? (y/n): ");
            string answer = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (answer != "y" && answer != "yes")
            {
                Console.WriteLine("No changes made.");
                return;
            }

            const string sql = "UPDATE Users SET RoleID = @RoleID WHERE UserID = @UserID;";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@RoleID", adminRoleId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.ExecuteNonQuery();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done — " + email + " is now an Admin.");
            Console.ResetColor();
        }

        private static void CreateNewAdmin(SqlConnection con, string email, int adminRoleId)
        {
            Console.Write("Full name: ");
            string fullName = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(fullName))
                throw new InvalidOperationException("Full name is required.");

            Console.Write("Phone (optional, press Enter to skip): ");
            string phoneInput = (Console.ReadLine() ?? "").Trim();
            object phone = string.IsNullOrWhiteSpace(phoneInput) ? (object)DBNull.Value : phoneInput;

            string password = PromptPassword();

            // ---- The part this whole file exists for ----
            // PasswordHelper here is the REAL class from Helpers/PasswordHelper.cs,
            // compiled directly into this .exe (see the compile command at the
            // top of this file) — not reimplemented, not copied by hand.
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.Hash(password, salt);

            const string sql = @"
                INSERT INTO Users (FullName, Email, PasswordHash, PasswordSalt, RoleID, PhoneNumber, IsActive, CreatedAt)
                VALUES (@FullName, @Email, @PasswordHash, @PasswordSalt, @RoleID, @PhoneNumber, 1, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int newUserId;
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                cmd.Parameters.AddWithValue("@RoleID", adminRoleId);
                cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                newUserId = (int)cmd.ExecuteScalar();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Admin account created — UserID " + newUserId + ", " + email + ".");
            Console.WriteLine("You can log in at Account/Login.aspx with this email and password now.");
            Console.ResetColor();
        }

        private static string PromptPassword()
        {
            while (true)
            {
                string password = ReadMasked("Password (at least " + MinPasswordLength + " characters): ");
                if (password.Length < MinPasswordLength)
                {
                    Console.WriteLine("  Too short — try again.");
                    continue;
                }

                string confirm = ReadMasked("Confirm password: ");
                if (password != confirm)
                {
                    Console.WriteLine("  Passwords didn't match — try again.");
                    continue;
                }

                return password;
            }
        }

        /// <summary>Masks input with '*' instead of echoing the real
        /// password to the console — the same reason a browser's password
        /// field is TextMode="Password", applied to a terminal.</summary>
        private static string ReadMasked(string prompt)
        {
            Console.Write(prompt);
            string result = "";

            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (result.Length > 0)
                    {
                        result = result.Substring(0, result.Length - 1);
                        Console.Write("\b \b");
                    }
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    result += key.KeyChar;
                    Console.Write("*");
                }
            }

            Console.WriteLine();
            return result;
        }
    }
}
