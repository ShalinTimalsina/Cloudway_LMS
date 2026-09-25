using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Account
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthBLL.IsLoggedIn)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
                LoadProfile();
        }

        private void LoadProfile()
        {
            User u = new UserBLL().GetById(AuthBLL.CurrentUserId);
            if (u == null) return; // shouldn't happen for a logged-in session, but never trust it silently

            txtEmail.Text = u.Email;
            txtFullName.Text = u.FullName;
            
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                new UserBLL().UpdateProfile(AuthBLL.CurrentUserId, txtFullName.Text, txtPhone.Text);
                litProfileMessage.Text = "<div class=\"alert alert-success\">Profile updated.</div>";
                AuthBLL.RefreshSessionName(txtFullName.Text.Trim());
            }
            catch (ValidationException vex)
            {
                litProfileMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
                LoadProfile(); // restore the read-only email field etc. after a failed postback
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                new AuthBLL().ChangePassword(AuthBLL.CurrentUserId, txtCurrentPassword.Text, txtNewPassword.Text);
                litPasswordMessage.Text = "<div class=\"alert alert-success\">Password changed.</div>";
                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmNewPassword.Text = "";
            }
            catch (ValidationException vex)
            {
                litPasswordMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }
    }
}
