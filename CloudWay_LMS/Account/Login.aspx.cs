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

                if (u.RoleID == 1)
                    Response.Redirect("~/Admin/Dashboard.aspx");
                else
                    Response.Redirect("~/Pages/Default.aspx");
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }
    }
}



