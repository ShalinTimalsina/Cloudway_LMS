namespace CloudWay_LMS.Models
{
    public class Quiz
    {
        public int    QuizID   { get; set; }
        public int    CourseID { get; set; }
        public string Title    { get; set; }
        public int    PassingScore { get; set; }
        

        // Joined / derived
        public string CourseTitle  { get; set; }
        public int    QuestionCount{ get; set; }
    }
}
