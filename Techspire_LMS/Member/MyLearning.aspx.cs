using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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