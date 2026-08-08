using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Techspire_LMS.BLL;
using Techspire_LMS.Models;

namespace Techspire_LMS.Admin
{
    public partial class ViewFeedback : System.Web.UI.Page
    {
        private readonly FeedbackBLL _bll = new FeedbackBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            var items = _bll.GetAll();
            gvFeedback.DataSource = items;
            gvFeedback.DataBind();
            litEmpty.Visible = items.Count == 0;
        }
        protected void gvFeedback_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFeedback.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        /// <summary>Bolds unread rows and hides the redundant "Mark read"
        /// button once a row is already read — small touch, but it's the
        /// difference between an admin inbox and a plain audit table.</summary>
        protected void gvFeedback_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            Feedback f = (Feedback)e.Row.DataItem;
            if (!f.IsRead)
            {
                e.Row.CssClass += " "; // hook for custom unread styling if you add one
                e.Row.Style["font-weight"] = "600";
            }

            LinkButton lnkMarkRead = (LinkButton)e.Row.FindControl("lnkMarkRead");
            lnkMarkRead.Visible = !f.IsRead;
        }

        protected void gvFeedback_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int feedbackId = int.Parse((string)e.CommandArgument);

            try
            {
                if (e.CommandName == "MarkRead")
                {
                    _bll.MarkRead(feedbackId);
                    litMessage.Text = "";
                }
                else if (e.CommandName == "DeleteRow")
                {
                    _bll.Remove(feedbackId);
                    litMessage.Text = "<div class=\"alert alert-success\">Feedback deleted.</div>";
                }
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }

            BindGrid();


        }
    }
}