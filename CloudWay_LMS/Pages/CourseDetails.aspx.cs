using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Pages
{
    public partial class CourseDetails : System.Web.UI.Page
    {
        
        // Identity for the enrol action comes from Session (AuthBLL), never from
        // the query string — the query string only ever carries the COURSE id,
        // which is public information. Using ?userid=.. here would be an
        // Insecure Direct Object Reference (see Phase 4's write-up on this).
        private int CourseId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        // Populated by BindEnrollmentState, read by IsLessonDone (called from
        // the lesson Repeater's markup) — computed once per page load rather
        // than re-querying per row.
        private Enrollment _currentEnrollment;
        private List<int> _completedLessonIds = new List<int>();

        protected Course CurrentCourse;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadCourse();
        }

        private void LoadCourse()
        {
            var courseBll = new CourseBLL();
            CurrentCourse = AuthBLL.IsAdmin ? courseBll.GetById(CourseId) : courseBll.GetPublishedById(CourseId);

            if (CurrentCourse == null)
            {
                phNotFound.Visible = true;
                phCourse.Visible = false;
                return;
            }

            litCategory.Text = Server.HtmlEncode(CurrentCourse.CategoryName);
            litTitle.Text = Server.HtmlEncode(CurrentCourse.Title);
            litMeta.Text = "Self-paced";
            litDescription.Text = Server.HtmlEncode(CurrentCourse.Description);

            if (!CurrentCourse.IsPublished)
                litMessage.Text = "<div class=\"alert alert-error\">This course is a draft — only admins can see it.</div>";

            rptTags.DataSource = new TagBLL().GetByCourse(CurrentCourse.CourseID);
            rptTags.DataBind();

            // Enrollment state first — the lesson list's checkmarks depend on it.
            BindEnrollmentState(CurrentCourse.CourseID);

            var lessons = new LessonBLL().GetByCourse(CurrentCourse.CourseID);
            rptLessons.DataSource = lessons;
            rptLessons.DataBind();
            litNoLessons.Visible = lessons.Count == 0;

            var quizzes = new QuizBLL().GetByCourse(CurrentCourse.CourseID);
            rptQuizzes.DataSource = quizzes;
            rptQuizzes.DataBind();
            litNoQuizzes.Visible = quizzes.Count == 0;
        }

        private void BindEnrollmentState(int courseId)
        {
            if (!AuthBLL.IsLoggedIn)
            {
                phGuestPrompt.Visible = true;
                phEnrolAction.Visible = false;
                phEnrolled.Visible = false;
                return;
            }

            var enrollmentBll = new EnrollmentBLL();
            _currentEnrollment = enrollmentBll.GetEnrollment(AuthBLL.CurrentUserId, courseId);

            if (_currentEnrollment != null)
            {
                phEnrolled.Visible = true;
                phEnrolAction.Visible = false;
                phGuestPrompt.Visible = false;
                litProgress.Text = _currentEnrollment.ProgressPercent.ToString("0");

                _completedLessonIds = enrollmentBll.GetCompletedLessonIds(_currentEnrollment.EnrollmentID);
            }
            else
            {
                phEnrolAction.Visible = true;
                phEnrolled.Visible = false;
                phGuestPrompt.Visible = false;
            }
        }

        /// <summary>Called from the lesson Repeater's markup — one lookup
        /// against the already-loaded completed-IDs list, no extra query
        /// per row.</summary>
        protected bool IsLessonDone(object lessonId)
        {
            return _completedLessonIds.Contains((int)lessonId);
        }

        protected void btnEnrol_Click(object sender, EventArgs e)
        {
            try
            {
                new EnrollmentBLL().Enroll(AuthBLL.CurrentUserId, CourseId);
                litMessage.Text = "<div class=\"alert alert-success\">You're now enrolled!</div>";
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }

            LoadCourse(); // refresh the enrol/progress state shown on the page
        }
    }
}
