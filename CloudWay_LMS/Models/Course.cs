using System;
using System.Collections.Generic;

namespace CloudWay_LMS.Models
{
    public class Course
    {
        // ---- Columns that map directly to the Courses table ----
        public int       CourseID         { get; set; }
        public string    Title            { get; set; }
        public string    Description      { get; set; }
        public int       CategoryID       { get; set; }
        public string    ThumbnailPath    { get; set; }
        public bool      IsPublished      { get; set; }
        public int       CreatedBy        { get; set; }
        public DateTime  CreatedAt        { get; set; }
        

        // ---- Joined / derived values, NOT stored in the Courses table ----
        public string     CategoryName   { get; set; }   // JOIN Categories
        public string     CreatedByName  { get; set; }   // JOIN Users
        public int        EnrolmentCount { get; set; }   // COUNT aggregate
        public List<Tag>  Tags           { get; set; }   // loaded separately via TagDAL.SelectByCourse

        // Duration removed per schema.

        public string ThumbnailOrDefault
        {
            get
            {
                return string.IsNullOrWhiteSpace(ThumbnailPath)
                     ? "https://via.placeholder.com/400x250.png?text=No+Thumbnail"
                     : ThumbnailPath;
            }
        }
    }
}
