using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;

/// <summary>
/// Every .aspx that sets MasterPageFile to Admin.master inherits this
/// enforcement automatically — that is the whole point of putting the check
/// HERE rather than copy-pasting it into every admin page's Page_Load. A new
/// admin page added next month is protected the moment it picks this master;
/// there is nothing to remember to add.
///
/// NOTE: this is a server-side redirect check backed by Session (via
/// AuthBLL), not ASP.NET Forms Authentication + folder &lt;authorization&gt;
/// rules. Adding an Admin/Web.config with &lt;deny users="*" /&gt;&lt;allow
/// roles="Admin" /&gt; on top of this is a good defence-in-depth exercise —
/// see the README for why we didn't wire Forms Authentication into this
/// template by default.
/// </summary>

namespace CloudWay_LMS.Masterpages
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthBLL.IsLoggedIn)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!AuthBLL.IsAdmin)
            {
                Response.Redirect("~/Pages/Default.aspx");
                return;
            }

            litAdminName.Text = Server.HtmlEncode(AuthBLL.CurrentUserName);
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            new AuthBLL().Logout();
            Response.Redirect("~/Pages/Default.aspx");
        }
    }
}