using System.Collections.Generic;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    public class LessonBLL
    {
        private readonly LessonDAL _dal = new LessonDAL();

        public List<Lesson> GetByCourse(int courseId) { return _dal.SelectByCourse(courseId); }
        public Lesson GetById(int lessonId) { return lessonId <= 0 ? null : _dal.SelectById(lessonId); }

        public int Add(Lesson l)
        {
            Validate(l);
            return _dal.Insert(l);
        }

        public void Edit(Lesson l)
        {
            if (l == null || l.LessonID <= 0) throw new ValidationException("Invalid lesson.");
            Validate(l);
            if (!_dal.Update(l)) throw new ValidationException("The lesson no longer exists.");
        }

        public void Remove(int lessonId)
        {
            if (lessonId <= 0) throw new ValidationException("Invalid lesson.");
            if (!_dal.Delete(lessonId)) throw new ValidationException("The lesson no longer exists.");
        }

        private void Validate(Lesson l)
        {
            if (l == null) throw new ValidationException("No lesson data was supplied.");
            if (l.CourseID <= 0) throw new ValidationException("Lesson must belong to a course.");
            if (string.IsNullOrWhiteSpace(l.Title)) throw new ValidationException("Lesson title is required.");
            l.Title = l.Title.Trim();
            if (l.Title.Length > 200) throw new ValidationException("Lesson title must be 200 characters or fewer.");
            if (l.SortOrder < 0) throw new ValidationException("Sort order cannot be negative.");
        }
    }
}
