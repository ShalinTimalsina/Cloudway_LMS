using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Helpers;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    public class TagBLL
    {
       
        private readonly TagDAL _dal = new TagDAL();

        public List<Tag> GetAll() { return _dal.SelectAll(); }
        public List<Tag> GetByCourse(int courseId) { return _dal.SelectByCourse(courseId); }
        public Tag GetById(int tagId) { return tagId <= 0 ? null : _dal.SelectById(tagId); }

        public int Add(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) throw new ValidationException("Tag name is required.");
            tagName = tagName.Trim();
            if (tagName.Length > 50) throw new ValidationException("Tag name must be 50 characters or fewer.");

            try
            {
                return _dal.Insert(tagName);
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, uniqueMessage: "That tag already exists.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "TagBLL.Add");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        public void Remove(int tagId)
        {
            if (tagId <= 0) throw new ValidationException("Invalid tag.");
            _dal.Delete(tagId); // CourseTags rows cascade — see CreateDatabase.sql
        }

        public void AttachToCourse(int courseId, int tagId)
        {
            if (courseId <= 0 || tagId <= 0) throw new ValidationException("Invalid course or tag.");
            try
            {
                _dal.AddTagToCourse(courseId, tagId);
            }
            catch (SqlException sqlEx)
            {
                // Duplicate pair is harmless — the tag is already on the course.
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) return;
                ErrorLogger.Log(sqlEx, "TagBLL.AttachToCourse");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        public void DetachFromCourse(int courseId, int tagId)
        {
            if (courseId <= 0 || tagId <= 0) throw new ValidationException("Invalid course or tag.");
            _dal.RemoveTagFromCourse(courseId, tagId);
        }
    }
}
