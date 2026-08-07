using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Techspire_LMS.BLL;

/// <summary>
/// Runs on every request for every page that uses this master.
/// Its only job is to show the right navigation for the current visitor —
/// all session/role logic lives in AuthBLL, never here directly.
/// </summary>
namespace Techspire_LMS.Masterpages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BuildNavigation();
        }

        private void BuildNavigation()
        {
            bool isLoggedIn = AuthBLL.IsLoggedIn;

            phGuest.Visible = !isLoggedIn;
            phLoggedIn.Visible = isLoggedIn;

            if (!isLoggedIn)
            {
                phAdmin.Visible = false;
                return;
            }

            // Server.HtmlEncode even though the name came from our own database —
            // defence in depth costs nothing and stops stored XSS via a crafted name.
            litWelcome.Text = Server.HtmlEncode(AuthBLL.CurrentUserName);
            phAdmin.Visible = AuthBLL.IsAdmin;
        }

        /// <summary>
        /// Lives on the master page so logout works identically from every page.
        /// Post/Redirect/Get pattern: redirect after the action so F5 doesn't repeat it.
        /// </summary>
        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            new AuthBLL().Logout();
            Response.Redirect("~/Pages/Default.aspx");
        }
    }
}