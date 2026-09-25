using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;

namespace CloudWay_LMS.Account
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            new AuthBLL().Logout();
            Response.Redirect("~/Pages/Default.aspx");
        }
    }
}