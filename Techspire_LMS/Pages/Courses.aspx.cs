using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Techspire_LMS.BLL;

namespace Techspire_LMS.Pages
{
    public partial class Courses : System.Web.UI.Page
    {
        private const int PageSize = 9;

        // ViewState (not Session) — this is page-local UI state, not
        // something that should follow the visitor to a different page or
        // survive across tabs the way Session-backed state would.
        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] == null ? 1 : (int)ViewState["CurrentPage"]; }
            set { ViewState["CurrentPage"] = value; }
        }

        // 0 means "no tag filter". Lives in ViewState (not the query string)
        // once set, so Prev/Next/Search postbacks keep the filter without
        // needing to thread it through every button's postback data.
        private int ActiveTagId
        {
            get { return ViewState["ActiveTagId"] == null ? 0 : (int)ViewState["ActiveTagId"]; }
            set { ViewState["ActiveTagId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategories();   // first load only — see Phase 2's #1 Web Forms bug

                // A tag link from CourseDetails.aspx arrives as ?tag=<id> —
                // only honoured on the very first load, exactly like any
                // other query-string-driven initial state in Web Forms.
                int tagId;
                if (int.TryParse(Request.QueryString["tag"], out tagId) && tagId > 0)
                    ActiveTagId = tagId;

                BindCourses();
            }
        }

        private void BindCategories()
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem("All categories", ""));
            foreach (var cat in new CategoryBLL().GetActive())
                ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem(cat.CategoryName, cat.CategoryID.ToString()));
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            CurrentPage = 1; // a new filter invalidates whatever page you were on
            BindCourses();
        }

        protected void lnkClearTag_Click(object sender, EventArgs e)
        {
            ActiveTagId = 0;
            CurrentPage = 1;
            BindCourses();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1) CurrentPage--;
            BindCourses();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            BindCourses();
        }

        private void BindCourses()
        {
            int? categoryId = string.IsNullOrEmpty(ddlCategory.SelectedValue)
                ? (int?)null : int.Parse(ddlCategory.SelectedValue);
            int? tagId = ActiveTagId > 0 ? ActiveTagId : (int?)null;

            if (tagId.HasValue)
            {
                var tag = new TagBLL().GetById(tagId.Value);
                phTagFilter.Visible = tag != null;
                if (tag != null) litActiveTag.Text = Server.HtmlEncode(tag.TagName);
                else ActiveTagId = 0; // tag was deleted since the link was created — drop the filter quietly
            }
            else
            {
                phTagFilter.Visible = false;
            }

            int totalCount;
            var courses = new CourseBLL().GetPublicPaged(
                categoryId, tagId, txtSearch.Text, CurrentPage, PageSize, out totalCount);

            rptCourses.DataSource = courses;
            rptCourses.DataBind();
            litEmpty.Visible = courses.Count == 0;

            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            phPager.Visible = totalPages > 1;
            btnPrev.Enabled = CurrentPage > 1;
            btnNext.Enabled = CurrentPage < totalPages;
            litPageStatus.Text = string.Format("Page {0} of {1} ({2} courses)",
                CurrentPage, Math.Max(totalPages, 1), totalCount);
        }
    }
}