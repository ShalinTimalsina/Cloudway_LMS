using System;

namespace Techspire_LMS.Models
{
    public class Enrollment
    {
        public int       EnrollmentID    { get; set; }
        public int       UserID          { get; set; }
        public int       CourseID        { get; set; }
        public DateTime  EnrolledAt      { get; set; }
        public decimal   ProgressPercent { get; set; }
        public DateTime? CompletedAt     { get; set; }

        // Joined / derived
        public string CourseTitle   { get; set; }
        public string ThumbnailPath { get; set; }
        public bool   IsCompleted { get { return CompletedAt.HasValue; } }
    }
}
