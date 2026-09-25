using System.Collections.Generic;
using System.Text.RegularExpressions;
using CloudWay_LMS.Data_Access_Layer;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.BLL
{
    public class FeedbackBLL
    {
        private readonly FeedbackDAL _dal = new FeedbackDAL();
        private static readonly Regex EmailPattern =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public List<Feedback> GetAll() { return _dal.SelectAll(); }

        /// <summary>currentUserId: pass 0 for a guest (not logged in) — the DAL
        /// stores that as SQL NULL, matching the nullable UserID column.</summary>
        public int Submit(Feedback f, int currentUserId)
        {
            Validate(f);
            f.UserID = currentUserId > 0 ? (int?)currentUserId : null;
            return _dal.Insert(f);
        }

        public void MarkRead(int feedbackId)
        {
            if (feedbackId <= 0) throw new ValidationException("Invalid feedback item.");
            if (!_dal.MarkRead(feedbackId)) throw new ValidationException("The feedback item no longer exists.");
        }

        public void Remove(int feedbackId)
        {
            if (feedbackId <= 0) throw new ValidationException("Invalid feedback item.");
            if (!_dal.Delete(feedbackId)) throw new ValidationException("The feedback item no longer exists.");
        }

        private void Validate(Feedback f)
        {
            if (f == null) throw new ValidationException("No feedback data was supplied.");

            if (string.IsNullOrWhiteSpace(f.Name)) throw new ValidationException("Name is required.");
            f.Name = f.Name.Trim();
            if (f.Name.Length > 100) throw new ValidationException("Name must be 100 characters or fewer.");

            if (string.IsNullOrWhiteSpace(f.Email) || !EmailPattern.IsMatch(f.Email.Trim()))
                throw new ValidationException("A valid email address is required.");
            f.Email = f.Email.Trim();

            if (string.IsNullOrWhiteSpace(f.Subject)) throw new ValidationException("Subject is required.");
            f.Subject = f.Subject.Trim();
            if (f.Subject.Length > 150) throw new ValidationException("Subject must be 150 characters or fewer.");

            if (string.IsNullOrWhiteSpace(f.Message)) throw new ValidationException("Message is required.");
            f.Message = f.Message.Trim();
        }
    }
}
