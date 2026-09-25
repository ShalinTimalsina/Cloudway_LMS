using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;

namespace CloudWay_LMS.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var u = new AuthBLL().Login(txtEmail.Text, txtPassword.Text);

                // Open redirect guard: only follow ReturnUrl if it's a LOCAL path.
                // Without this check, a link like Login.aspx?ReturnUrl=https://evil.example
                // would send a just-authenticated user straight to a phishing site.
                string returnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl) && Server.UrlDecode(returnUrl).StartsWith("/"))
                    Response.Redirect(returnUrl);
                else
                    
                if (u.RoleID == 1)
                    Response.Redirect("~/Admin/ManageCategories.aspx");
                else
                    Response.Redirect("~/Member/MyLearning.aspx");
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }
    }
}

