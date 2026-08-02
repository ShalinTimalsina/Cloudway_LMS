using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Techspire_LMS.Models;
using Techspire_LMS.Data_Access_Layer;
namespace Techspire_LMS
{
    public partial class TestForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           UserDAL userDAL = new UserDAL();
            List<User> users = userDAL.SelectAll();
            // Bind the list of users to the GridView
            foreach (User user in users)
            {
                Response.Write($" Name: {user.FullName}, Email: {user.Email}</p>");
                //test
            }
        }
    }
}