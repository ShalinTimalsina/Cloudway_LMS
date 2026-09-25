namespace CloudWay_LMS.Models
{
    public class Quiz
    {
        public int    QuizID   { get; set; }
        public int    CourseID { get; set; }
        public string Title    { get; set; }
        public int    PassingScore { get; set; }
        public string Description  { get; set; }
        public int?   TimeLimitMinutes { get; set; }
        public bool   IsPublished  { get; set; }
        

        // Joined / derived
        public string CourseTitle  { get; set; }
        public int    QuestionCount{ get; set; }
    }
}
