using System.Collections.Generic;
using CloudWay_LMS.Data_Access_Layer;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.BLL
{
    /// <summary>The generic per-lesson attachment (code file, PCAP, slide deck,
    /// cheat sheet...). This class validates the ROW, not the file upload itself
    /// — file-size/extension checks belong in the page, right where FileUpload
    /// lives, per the layering rule "no System.Web in the BLL".</summary>
    public class ResourceBLL
    {
        private readonly ResourceDAL _dal = new ResourceDAL();

        public List<Resource> GetByLesson(int lessonId) { return _dal.SelectByLesson(lessonId); }

        public int Add(Resource r)
        {
            Validate(r);
            return _dal.Insert(r);
        }

        public void Remove(int resourceId)
        {
            if (resourceId <= 0) throw new ValidationException("Invalid resource.");
            if (!_dal.Delete(resourceId)) throw new ValidationException("The resource no longer exists.");
        }

        private void Validate(Resource r)
        {
            if (r == null) throw new ValidationException("No resource data was supplied.");
            if (r.LessonID <= 0) throw new ValidationException("Resource must belong to a lesson.");
            if (string.IsNullOrWhiteSpace(r.FileName)) throw new ValidationException("Resource title is required.");
            if (string.IsNullOrWhiteSpace(r.FilePath)) throw new ValidationException("Resource file path is required.");
            
        }
    }
}
