using System;
using System.Security.Cryptography;
using System.Text;

namespace Techspire_LMS.Helpers
{
    /// <summary>
    /// Salted-hash password storage. We NEVER store a plain password, and we
    /// NEVER hash without a per-user salt — a plain hash lets an attacker who
    /// steals the Users table pre-compute ("rainbow table") every common
    /// password once and match it against every user at the same time. A
    /// unique salt per user defeats that: the same password produces a
    /// different hash for every account.
    ///
    /// This uses SHA-256, matching the CHAR(64)/CHAR(32) column sizes from the
    /// database design. For a production system, prefer a slow, purpose-built
    /// algorithm (PBKDF2, BCrypt, Argon2) — see the note in AuthBLL.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>Generates 16 random bytes, returned as a 32-character hex string.</summary>
        public static string GenerateSalt()
        {
            byte[] bytes = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return ToHex(bytes);
        }

        /// <summary>SHA-256(password + salt), returned as a 64-character hex string.</summary>
        public static string Hash(string password, string salt)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] combined = Encoding.UTF8.GetBytes((password ?? "") + salt);
                byte[] hash = sha.ComputeHash(combined);
                return ToHex(hash);
            }
        }

        /// <summary>Re-hashes the supplied password with the stored salt and compares.</summary>
        public static bool Verify(string password, string salt, string expectedHash)
        {
            string actualHash = Hash(password, salt);
            // Ordinal comparison: hex digits only, no culture-specific casing rules apply.
            return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string ToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
