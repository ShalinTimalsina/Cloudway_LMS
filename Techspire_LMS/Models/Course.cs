using System;
using System.Collections.Generic;

namespace Techspire_LMS.Models
{
    public class Course
    {
        // ---- Columns that map directly to the Courses table ----
        public int       CourseID         { get; set; }
        public string    Title            { get; set; }
        public string    ShortDescription { get; set; }
        public string    FullDescription  { get; set; }
        public int       CategoryID       { get; set; }
        public string    ThumbnailPath    { get; set; }
        public string    DifficultyLevel  { get; set; }
        public int?      DurationMinutes  { get; set; }
        public bool      IsPublished      { get; set; }
        public int       CreatedBy        { get; set; }
        public DateTime  CreatedAt        { get; set; }
        public DateTime  UpdatedAt        { get; set; }

        // ---- Joined / derived values, NOT stored in the Courses table ----
        public string     CategoryName   { get; set; }   // JOIN Categories
        public string     CreatedByName  { get; set; }   // JOIN Users
        public int        EnrolmentCount { get; set; }   // COUNT aggregate
        public List<Tag>  Tags           { get; set; }   // loaded separately via TagDAL.SelectByCourse

        public string DurationDisplay
        {
            get
            {
                if (!DurationMinutes.HasValue) return "—";
                int h = DurationMinutes.Value / 60;
                int m = DurationMinutes.Value % 60;
                return h > 0 ? string.Format("{0}h {1}m", h, m) : string.Format("{0}m", m);
            }
        }

        public string ThumbnailOrDefault
        {
            get
            {
                return string.IsNullOrWhiteSpace(ThumbnailPath)
                     ? "~/Content/images/placeholder.png"
                     : ThumbnailPath;
            }
        }
    }
}
