using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Admin
{
    public partial class ManageCategories : System.Web.UI.Page
    {
      
    private readonly CategoryBLL _bll = new CategoryBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvCategories.DataSource = _bll.GetAll();
            gvCategories.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Category c = new Category
                {
                    CategoryID = int.Parse(hfCategoryId.Value),
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    
                };

                if (c.CategoryID == 0)
                {
                    _bll.Add(c);
                    litMessage.Text = "<div class=\"alert alert-success\">Category added.</div>";
                }
                else
                {
                    _bll.Edit(c);
                    litMessage.Text = "<div class=\"alert alert-success\">Category updated.</div>";
                }

                ResetForm();
                BindGrid();
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                Category c = _bll.GetById(categoryId);
                if (c == null) return;

                hfCategoryId.Value = c.CategoryID.ToString();
                txtName.Text = c.Name;
                txtDescription.Text = c.Description;
                
                litFormTitle.Text = "Edit category";
            }
            else if (e.CommandName == "DeleteRow")
            {
                try
                {
                    _bll.Remove(categoryId);
                    litMessage.Text = "<div class=\"alert alert-success\">Category deleted.</div>";
                }
                catch (ValidationException vex)
                {
                    // e.g. "This category still has courses assigned to it..."
                    litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
                }
                BindGrid();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
        protected void gvCategories_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategories.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void ResetForm()
        {
            hfCategoryId.Value = "0";
            txtName.Text = "";
            txtDescription.Text = "";
            
            litFormTitle.Text = "Add a category";
        }
    }
}

