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
    public partial class LessonDetails : System.Web.UI.Page
    {
        private int LessonId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        // Public/protected so the markup's <%= %> runtime expressions
        // (not <%# %> data-binding expressions, which need an explicit
        // DataBind() call) can read them directly.
        protected int CourseId 
        { 
            get { return ViewState["CourseId"] != null ? (int)ViewState["CourseId"] : 0; }
            private set { ViewState["CourseId"] = value; } 
        }
        
        protected string VideoUrl 
        { 
            get { return (string)ViewState["VideoUrl"]; }
            private set { ViewState["VideoUrl"] = value; } 
        }

        protected string BreadcrumbTitle { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadLesson();
        }

        private void LoadLesson()
        {
            Lesson lesson = new LessonBLL().GetById(LessonId);
            if (lesson == null)
            {
                phNotFound.Visible = true;
                phLesson.Visible = false;
                return;
            }

            CourseId = lesson.CourseID;
            Course course = new CourseBLL().GetById(CourseId);
            bool courseVisible = course != null && (course.IsPublished || AuthBLL.IsAdmin);

            if (!courseVisible)
            {
                phNotFound.Visible = true;
                phLesson.Visible = false;
                return;
            }

            // Gate the CONTENT, not just the page — a logged-in but
            // non-enrolled visitor (and any guest) sees the enrol prompt
            // instead of the lesson body. Admins can always preview.
            Enrollment enrollment = AuthBLL.IsLoggedIn
                ? new EnrollmentBLL().GetEnrollment(AuthBLL.CurrentUserId, CourseId)
                : null;
            bool hasAccess = enrollment != null || AuthBLL.IsAdmin;

            litCourseTitle.Text = Server.HtmlEncode(course.Title);
            litTitle.Text = Server.HtmlEncode(lesson.Title);
            BreadcrumbTitle = Server.HtmlEncode(lesson.Title);
            litMeta.Text = "";

            if (!hasAccess)
            {
                litContent.Text = "";
                phEnrolPrompt.Visible = true;
                phMarkComplete.Visible = false;
                phCompleted.Visible = false;
                phVideo.Visible = false;
                rptResources.DataSource = null;
                rptResources.DataBind();
                return;
            }

            // See the HTML-encoding note in the markup: lesson content is
            // admin-authored only, so it's rendered as-is, not encoded.
            litContent.Text = "<div style=\"background: white; padding: 2.5rem 3rem; border-radius: var(--radius); box-shadow: var(--shadow); margin-top: 2rem; border: 1px solid var(--border); font-size: 1.1rem; line-height: 1.7; color: var(--text);\">" + lesson.Content + "</div>";

            if (!string.IsNullOrWhiteSpace(lesson.VideoUrl))
            {
                VideoUrl = lesson.VideoUrl;
                phVideo.Visible = true;
                ShowVideoByKind(lesson.VideoUrl);
            }

            var resources = new ResourceBLL().GetByLesson(lesson.LessonID);
            rptResources.DataSource = resources;
            rptResources.DataBind();
            litNoResources.Visible = resources.Count == 0;

            if (enrollment != null)
            {
                bool isComplete = new EnrollmentBLL().IsLessonComplete(enrollment.EnrollmentID, lesson.LessonID);
                phCompleted.Visible = isComplete;
                phMarkComplete.Visible = !isComplete;
            }
            else
            {
                // Admin previewing without being enrolled — nothing to mark.
                phCompleted.Visible = false;
                phMarkComplete.Visible = false;
            }

            BindPrevNext(lesson);
        }

        /// <summary>Finds the current lesson's position in its course's
        /// SortOrder sequence and points Prev/Next at its neighbours.</summary>
        private void BindPrevNext(Lesson current)
        {
            List<Lesson> all = new LessonBLL().GetByCourse(CourseId);
            int index = all.FindIndex(l => l.LessonID == current.LessonID);
            if (index < 0) return;

            if (index > 0)
            {
                lnkPrev.NavigateUrl = ResolveUrl("~/Pages/LessonDetails.aspx?id=" + all[index - 1].LessonID);
                lnkPrev.Visible = true;
            }
            if (index < all.Count - 1)
            {
                lnkNext.NavigateUrl = ResolveUrl("~/Pages/LessonDetails.aspx?id=" + all[index + 1].LessonID);
                lnkNext.Text = "Next Lesson &rarr;";
                lnkNext.CssClass = "btn btn-outline";
                lnkNext.Visible = true;
            }
            else
            {
                lnkNext.NavigateUrl = ResolveUrl("~/Pages/CourseDetails.aspx?id=" + CourseId);
                lnkNext.Text = "Course Overview &rarr;";
                lnkNext.CssClass = "btn"; // Solid button to indicate completion
                lnkNext.Visible = true;
            }
        }

        /// <summary>
        /// Picks how to render a lesson's VideoUrl: a real &lt;video&gt; tag
        /// for a direct file link, a real &lt;iframe&gt; for an embed-ready
        /// URL (YouTube/Vimeo's own "embed" link format — NOT their regular
        /// "watch"/share links, which refuse to load in an iframe), or a
        /// plain link as the fallback for anything else. This is what makes
        /// UC7 ("view lesson + video") an actual embedded player instead of
        /// just a link out — worth knowing if you add another video host:
        /// this method is the only place that needs a new pattern.
        /// </summary>
        private void ShowVideoByKind(string url)
        {
            string lower = url.ToLowerInvariant();
            string[] fileExtensions = { ".mp4", ".webm", ".ogg", ".ogv" };

            if (fileExtensions.Any(ext => lower.EndsWith(ext)))
            {
                phVideoFile.Visible = true;
            }
            else if (lower.Contains("youtube.com/embed/") || lower.Contains("player.vimeo.com/video/"))
            {
                phVideoEmbed.Visible = true;
            }
            else
            {
                phVideoLink.Visible = true;
            }
        }

        protected void btnMarkComplete_Click(object sender, EventArgs e)
        {
            try
            {
                decimal newProgress = new EnrollmentBLL().MarkLessonComplete(AuthBLL.CurrentUserId, CourseId, LessonId);
                litMessage.Text = "<div class=\"alert alert-success\">Marked complete - course progress is now "
                    + newProgress.ToString("0") + "%.</div>";
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }

            LoadLesson();
        }




    }
}

