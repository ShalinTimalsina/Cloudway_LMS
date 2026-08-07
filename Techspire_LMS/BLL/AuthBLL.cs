using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Helpers;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    /// <summary>
    /// Registration, login, session and password-change rules. This is the
    /// only class that touches Session directly — pages call AuthBLL, never
    /// Session, so the "how are we tracking who's logged in" decision lives
    /// in exactly one place.
    ///
    /// PRODUCTION NOTE: this uses salted SHA-256 (see PasswordHelper) to match
    /// the CHAR(64)/CHAR(32) columns from the teaching pack's database design.
    /// A real deployment should move to a slow, purpose-built algorithm
    /// (PBKDF2 via System.Security.Cryptography.Rfc2898DeriveBytes, or
    /// BCrypt.Net) — SHA-256 is fast, which is exactly the wrong property for
    /// password hashing (fast = cheap to brute-force). Worth a sentence in
    /// your report's "future enhancements" section.
    /// </summary>
    public class AuthBLL
    {
        private readonly UserDAL _userDal = new UserDAL();

        private const int DefaultMemberRoleId = 2; // matches the Roles seed data — "Member"
        private static readonly Regex EmailPattern =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        // Login lockout — a lightweight defence against password guessing,
        // not a substitute for a real rate limiter (which would also cover
        // an attacker spreading guesses across many accounts, not just
        // hammering one). Five wrong passwords locks the ACCOUNT for 15
        // minutes; the counter resets to zero on either a successful login
        // or a lockout being triggered, so the next window starts fresh.
        private const int MaxFailedAttempts = 5;
        private static readonly System.TimeSpan LockoutDuration = System.TimeSpan.FromMinutes(15);

        // ================================================================
        // REGISTER
        // ================================================================
        public int Register(string fullName, string email, string password, string phoneNumber)
        {
            ValidateRegistration(fullName, email, password);

            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.Hash(password, salt);

            User u = new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleID = DefaultMemberRoleId,
                PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim()
            };

            try
            {
                return _userDal.Insert(u);
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, uniqueMessage: "That email address is already registered.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "AuthBLL.Register");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        private void ValidateRegistration(string fullName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ValidationException("Full name is required.");
            if (fullName.Trim().Length > 100)
                throw new ValidationException("Full name must be 100 characters or fewer.");

            if (string.IsNullOrWhiteSpace(email) || !EmailPattern.IsMatch(email.Trim()))
                throw new ValidationException("A valid email address is required.");

            // Length rule only — never enforce "must contain a symbol" style
            // rules server-side beyond a minimum; they push users toward
            // weaker, more predictable passwords than a plain length floor.
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                throw new ValidationException("Password must be at least 8 characters.");
        }

        // ================================================================
        // LOGIN
        // ================================================================
        /// <summary>
        /// Throws the SAME message for "no such email" and "wrong password"
        /// — telling an attacker which one it was is a user-enumeration
        /// leak. The lockout message below is a DELIBERATE, narrower
        /// exception to that rule: once someone is locked out, saying so
        /// (rather than still claiming "invalid email or password") is more
        /// honest to a real user who's about to be confused why their
        /// correct password "isn't working" — and it only reveals that SOME
        /// account with that email exists and has recently failed several
        /// times, not which one of your specific guesses was wrong. That's
        /// a reasonable trade worth naming explicitly rather than making
        /// silently — most real-world systems (GitHub included) make the
        /// same call.
        /// </summary>
        public User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
                throw new ValidationException("Email and password are required.");

            User u = _userDal.SelectByEmail(email.Trim().ToLowerInvariant());

            if (u == null || !u.IsActive)
                throw new ValidationException("Invalid email or password.");

            if (u.LockoutEndUtc.HasValue && u.LockoutEndUtc.Value > System.DateTime.UtcNow)
            {
                int minutesLeft = (int)System.Math.Ceiling((u.LockoutEndUtc.Value - System.DateTime.UtcNow).TotalMinutes);
                throw new ValidationException(string.Format(
                    "Too many failed attempts. Try again in {0} minute{1}.",
                    minutesLeft, minutesLeft == 1 ? "" : "s"));
            }

            if (!PasswordHelper.Verify(password, u.PasswordSalt, u.PasswordHash))
            {
                int newCount = u.FailedLoginAttempts + 1;
                System.DateTime? lockoutEnd = null;

                if (newCount >= MaxFailedAttempts)
                {
                    lockoutEnd = System.DateTime.UtcNow.Add(LockoutDuration);
                    newCount = 0; // starts counting fresh once the lockout expires
                }

                _userDal.RecordFailedLogin(u.UserID, newCount, lockoutEnd);
                throw new ValidationException("Invalid email or password.");
            }

            _userDal.ResetFailedLogins(u.UserID);
            _userDal.UpdateLastLogin(u.UserID);
            SetSession(u);
            return u;
        }

        public void Logout()
        {
            if (HttpContext.Current == null) return;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        private void SetSession(User u)
        {
            HttpContext.Current.Session["UserID"] = u.UserID;
            HttpContext.Current.Session["FullName"] = u.FullName;
            HttpContext.Current.Session["RoleID"] = u.RoleID;
            HttpContext.Current.Session["RoleName"] = u.RoleName;
        }

        /// <summary>Called after a profile edit changes the display name —
        /// the nav's "Hi, <name>" (see Site.master.cs) reads Session, so a
        /// name change needs to update Session too or the header would keep
        /// showing the OLD name until the next login. Kept here, not in
        /// Profile.aspx.cs, so Session access stays in exactly one class.</summary>
        public static void RefreshSessionName(string fullName)
        {
            if (IsLoggedIn) HttpContext.Current.Session["FullName"] = fullName;
        }

        // ================================================================
        // SESSION READ HELPERS — call these from any page or master page
        // ================================================================
        public static bool IsLoggedIn
        {
            get { return HttpContext.Current != null && HttpContext.Current.Session["UserID"] != null; }
        }

        public static int CurrentUserId
        {
            get { return IsLoggedIn ? (int)HttpContext.Current.Session["UserID"] : 0; }
        }

        public static string CurrentUserName
        {
            get { return IsLoggedIn ? (string)HttpContext.Current.Session["FullName"] : null; }
        }

        public static bool IsAdmin
        {
            get { return IsLoggedIn && (int)HttpContext.Current.Session["RoleID"] == 1; }
        }

        /// <summary>Call at the top of any page that requires a signed-in user.</summary>
        public static void RequireLogin()
        {
            if (!IsLoggedIn) throw new ValidationException("Please log in to continue.");
        }

        /// <summary>Call at the top of any admin-only page or admin-only action.</summary>
        public static void RequireAdmin()
        {
            if (!IsAdmin) throw new ValidationException("Administrator access is required for this action.");
        }

        // ================================================================
        // CHANGE PASSWORD
        // ================================================================
        public void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            User u = _userDal.SelectById(userId);
            if (u == null) throw new ValidationException("User not found.");

            if (!PasswordHelper.Verify(currentPassword, u.PasswordSalt, u.PasswordHash))
                throw new ValidationException("Current password is incorrect.");

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
                throw new ValidationException("New password must be at least 8 characters.");

            string newSalt = PasswordHelper.GenerateSalt();
            string newHash = PasswordHelper.Hash(newPassword, newSalt);
            _userDal.ChangePassword(userId, newHash, newSalt);
        }
    }
}
