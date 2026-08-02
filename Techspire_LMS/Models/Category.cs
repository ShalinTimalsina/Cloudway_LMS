namespace Techspire_LMS.Models
{
    public class Category
    {
        public int    CategoryID   { get; set; }
        public string CategoryName { get; set; }
        public string Description  { get; set; }
        public bool   IsActive     { get; set; }

        // Joined / derived
        public int CourseCount { get; set; }   // COUNT aggregate, admin screen
    }
}
