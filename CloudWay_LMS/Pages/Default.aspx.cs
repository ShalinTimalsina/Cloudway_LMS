using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;

namespace CloudWay_LMS.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var courses = new CourseBLL().GetFeatured(6);
                rptFeatured.DataSource = courses;
                rptFeatured.DataBind();
                litEmpty.Visible = courses.Count == 0;
            }
        }
    }
}