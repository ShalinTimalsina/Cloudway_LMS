using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;

namespace CloudWay_LMS.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                new AuthBLL().Register(txtFullName.Text, txtEmail.Text, txtPassword.Text, txtPhone.Text);
                new AuthBLL().Login(txtEmail.Text, txtPassword.Text); // sign them straight in
                Response.Redirect("~/Pages/Default.aspx");
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }
    }
}