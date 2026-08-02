namespace Techspire_LMS.Models
{
    public class Lesson
    {
        public int    LessonID        { get; set; }
        public int    CourseID        { get; set; }
        public string Title           { get; set; }
        public string ContentHtml     { get; set; }
        public string VideoUrl        { get; set; }
        public int    SortOrder       { get; set; }
        public int?   DurationMinutes { get; set; }
    }
}
