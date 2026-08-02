using System;

namespace Techspire_LMS.Models
{
    public class Feedback
    {
        public int      FeedbackID  { get; set; }
        public int?     UserID      { get; set; }   // NULL when a guest submits
        public string   Name        { get; set; }
        public string   Email       { get; set; }
        public string   Subject     { get; set; }
        public string   Message     { get; set; }
        public bool     IsRead      { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
