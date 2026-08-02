namespace Techspire_LMS.Models
{
    /// <summary>
    /// Free-form label attached to Courses via the CourseTags junction table.
    /// This is the extensibility hook: a programming platform seeds
    /// "Python"/"OOP", a cybersecurity platform seeds "CTF"/"OWASP" — same
    /// model and table either way.
    /// </summary>
    public class Tag
    {
        public int    TagID   { get; set; }
        public string TagName { get; set; }
    }
}
