using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudWay_LMS.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected int TotalUsers { get; set; }
        protected int TotalCourses { get; set; }
        protected int TotalCategories { get; set; }
        protected int TotalFeedback { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TotalUsers = new BLL.UserBLL().GetAll().Count;
                TotalCourses = new BLL.CourseBLL().GetAllForAdmin().Count;
                TotalCategories = new BLL.CategoryBLL().GetAll().Count;
                TotalFeedback = new BLL.FeedbackBLL().GetAll().Count;
            }
        }
    }
}