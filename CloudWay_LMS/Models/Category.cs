namespace CloudWay_LMS.Models
{
    public class Category
    {
        public int    CategoryID   { get; set; }
        public string Name { get; set; }
        public string Description  { get; set; }
        

        // Joined / derived
        public int CourseCount { get; set; }   // COUNT aggregate, admin screen
    }
}
