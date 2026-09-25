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
    public partial class Contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Pre-fill for a logged-in visitor — one less thing to type,
            // and it's the same info AuthBLL already has in Session.
            if (!IsPostBack && AuthBLL.IsLoggedIn)
            {
                txtName.Text = AuthBLL.CurrentUserName;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Feedback f = new Feedback
                {
                    Name = txtName.Text,
                    Email = txtEmail.Text,
                    Subject = txtSubject.Text,
                    Message = txtMessage.Text
                };

                // 0 tells FeedbackBLL.Submit "guest" — stored as SQL NULL on
                // the nullable UserID column either way.
                new FeedbackBLL().Submit(f, AuthBLL.IsLoggedIn ? AuthBLL.CurrentUserId : 0);

                phForm.Visible = false;
                phThanks.Visible = true;
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }
    }
}