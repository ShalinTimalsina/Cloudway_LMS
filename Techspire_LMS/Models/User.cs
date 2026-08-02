using System;

namespace Techspire_LMS.Models
{
    public class User
    {
        public int       UserID       { get; set; }
        public string    FullName     { get; set; }
        public string    Email        { get; set; }
        public string    PasswordHash { get; set; }
        public string    PasswordSalt { get; set; }
        public int       RoleID       { get; set; }
        public string    PhoneNumber  { get; set; }
        public bool      IsActive     { get; set; }
        public DateTime  CreatedAt    { get; set; }
        public DateTime? LastLoginAt  { get; set; }

        // Joined / derived — not stored in the Users table
        public string RoleName { get; set; }
        public bool   IsAdmin  { get { return RoleID == 1; } }
    }
}
