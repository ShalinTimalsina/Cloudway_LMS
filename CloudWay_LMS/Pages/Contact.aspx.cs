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
            if (!IsPostBack)
            {
                if (AuthBLL.IsLoggedIn)
                {
                    txtName.Text = AuthBLL.CurrentUserName;
                }
                
                if (!string.IsNullOrEmpty(Request.QueryString["subject"]))
                {
                    txtSubject.Text = Request.QueryString["subject"];
                }
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