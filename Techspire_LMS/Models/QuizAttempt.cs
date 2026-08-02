using System;

namespace Techspire_LMS.Models
{
    public class QuizAttempt
    {
        public int      AttemptID   { get; set; }
        public int      UserID      { get; set; }
        public int      QuizID      { get; set; }
        public int      Score       { get; set; }
        public int      TotalMarks  { get; set; }  // snapshot at attempt time
        public bool     IsPassed    { get; set; }
        public DateTime AttemptedAt { get; set; }

        // Joined / derived
        public string QuizTitle { get; set; }

        public decimal PercentScore
        {
            get { return TotalMarks == 0 ? 0m : Math.Round((decimal)Score * 100 / TotalMarks, 2); }
        }
    }
}
