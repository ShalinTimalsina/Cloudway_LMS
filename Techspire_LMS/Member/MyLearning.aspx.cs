using System;
using Techspire_LMS.BLL;

namespace Techspire_LMS.Member
{
    public partial class MyLearning : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthBLL.IsLoggedIn)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
            {
                BindEnrollments();
                BindAttempts();
            }
        }

        private void BindEnrollments()
        {
            var enrollments = new EnrollmentBLL().GetByUser(AuthBLL.CurrentUserId);
            rptEnrollments.DataSource = enrollments;
            rptEnrollments.DataBind();
            litNoEnrollments.Visible = enrollments.Count == 0;
        }

        /// <summary>enrollmentId comes from the button's CommandArgument —
        /// client-controlled, so it is NOT trusted alone. AuthBLL.CurrentUserId
        /// (from Session) is what actually proves ownership; see the IDOR
        /// note on EnrollmentBLL.Unenroll for why both are required.</summary>
        protected void rptEnrollments_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Unenroll") return;

            int enrollmentId = int.Parse((string)e.CommandArgument);

            try
            {
                new EnrollmentBLL().Unenroll(AuthBLL.CurrentUserId, enrollmentId);
            }
            catch (ValidationException)
            {
                // Enrolment already gone, or didn't belong to this user —
                // either way, the grid below is about to re-show reality.
            }

            BindEnrollments();
        }

        private void BindAttempts()
        {
            var attempts = new QuizBLL().GetAttemptsByUser(AuthBLL.CurrentUserId);
            gvAttempts.DataSource = attempts;
            gvAttempts.DataBind();
            litNoAttempts.Visible = attempts.Count == 0;
        }

        /// <summary>Enrollment (unlike Course) has no computed "or default"
        /// thumbnail property of its own — this fills the same role for the
        /// enrollment Repeater's markup.</summary>
        protected string ThumbnailOrDefault(string thumbnailPath)
        {
            return string.IsNullOrWhiteSpace(thumbnailPath)
                ? "~/Content/images/placeholder.png"
                : thumbnailPath;
        }
    }
}