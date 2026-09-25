using System;
using System.IO;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Admin
{
    public partial class ManageCourses : System.Web.UI.Page
    {
        private readonly CourseBLL _bll = new CourseBLL();

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const int MaxThumbnailBytes = 2 * 1024 * 1024; // 2 MB
        private const string ThumbnailUploadFolder = "~/Content/uploads/thumbnails/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategoryDropdown();
                BindGrid();
            }
        }

        /// <summary>
        /// Deliberately GetAll(), not GetActive(). The public catalogue
        /// (Pages/Courses.aspx) correctly hides inactive categories from
        /// guests — but THIS dropdown also has to represent whatever
        /// category an EXISTING course already has, even if that category
        /// was deactivated after the course was created. Using GetActive()
        /// here means editing such a course throws
        /// ArgumentOutOfRangeException the moment gvCourses_RowCommand
        /// tries to set SelectedValue to a CategoryID that isn't in the
        /// list — the value has to exist among the Items for
        /// SelectedValue to succeed. Showing every category (with inactive
        /// ones labelled) is what keeps editing old data possible.
        /// </summary>
        private void BindCategoryDropdown()
        {
            ddlCategory.Items.Clear();
            foreach (var cat in new CategoryBLL().GetAll())
            {
                string text = cat.Name;
                ddlCategory.Items.Add(new ListItem(text, cat.CategoryID.ToString()));
            }
        }

        private void BindGrid()
        {
            gvCourses.DataSource = _bll.GetAllForAdmin();
            gvCourses.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // A validation failure here (bad extension, too large) throws
                // BEFORE anything touches the database — same "validate first"
                // rule as every other BLL write, just enforced at the file level.
                string uploadedPath = UploadThumbnailIfProvided();

                Course c = new Course
                {
                    CourseID = int.Parse(hfCourseId.Value),
                    Title = txtTitle.Text,
                    Description = txtTitle.Text,
                    CategoryID = int.Parse(ddlCategory.SelectedValue),
                    // A newly uploaded file wins; otherwise keep whatever path
                    // was already on the course (shown read-only in txtThumbnail).
                    ThumbnailPath = uploadedPath ?? txtThumbnail.Text,
                    IsPublished = chkIsPublished.Checked
                };

                if (c.CourseID == 0)
                {
                    c.CreatedBy = AuthBLL.CurrentUserId;
                    _bll.Add(c, callerIsAdmin: true);
                    litMessage.Text = "<div class=\"alert alert-success\">Course added.</div>";
                }
                else
                {
                    _bll.Edit(c, callerIsAdmin: true);
                    litMessage.Text = "<div class=\"alert alert-success\">Course updated.</div>";
                }

                ResetForm();
                BindGrid();
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }

        /// <summary>Returns the new ~/Content/uploads/thumbnails/... path if a
        /// file was chosen, or null if the FileUpload was left empty (meaning
        /// "keep the existing thumbnail"). Throws ValidationException — the
        /// same type every BLL method throws — so btnSave_Click's existing
        /// catch block handles a bad upload exactly like any other bad input,
        /// with no separate error-handling path to maintain.</summary>
        private string UploadThumbnailIfProvided()
        {
            if (!fuThumbnail.HasFile) return null;

            string extension = Path.GetExtension(fuThumbnail.FileName).ToLowerInvariant();
            if (Array.IndexOf(AllowedImageExtensions, extension) < 0)
                throw new ValidationException("Thumbnail must be a JPG, PNG, GIF or WEBP image.");

            if (fuThumbnail.PostedFile.ContentLength > MaxThumbnailBytes)
                throw new ValidationException("Thumbnail must be 2 MB or smaller.");

            string folderPath = Server.MapPath(ThumbnailUploadFolder);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // A random filename — never the visitor-supplied one — sidesteps
            // both path traversal (a filename like "..\..\web.config") and
            // filename collisions between two different admins' uploads.
            string safeFileName = Guid.NewGuid().ToString("N") + extension;
            fuThumbnail.SaveAs(Path.Combine(folderPath, safeFileName));

            return ThumbnailUploadFolder + safeFileName;
        }

        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int courseId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                Course c = _bll.GetById(courseId);
                if (c == null) return;

                hfCourseId.Value = c.CourseID.ToString();
                txtTitle.Text = c.Title;
                txtTitle.Text = c.Description;
                ddlCategory.SelectedValue = c.CategoryID.ToString();
                txtThumbnail.Text = c.ThumbnailPath;
                chkIsPublished.Checked = c.IsPublished;
                litFormTitle.Text = "Edit course";

                bool hasThumbnail = !string.IsNullOrWhiteSpace(c.ThumbnailPath);
                imgThumbnailPreview.Visible = hasThumbnail;
                if (hasThumbnail) imgThumbnailPreview.ImageUrl = c.ThumbnailPath;
            }
            else if (e.CommandName == "DeleteRow")
            {
                try
                {
                    _bll.Remove(courseId);
                    litMessage.Text = "<div class=\"alert alert-success\">Course deleted.</div>";
                }
                catch (ValidationException vex)
                {
                    // e.g. "This course still has learners enrolled..."
                    litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
                }
                BindGrid();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            hfCourseId.Value = "0";
            txtTitle.Text = "";
            txtTitle.Text = "";
            txtThumbnail.Text = "";
            chkIsPublished.Checked = false;
            imgThumbnailPreview.Visible = false;
            litFormTitle.Text = "Add a course";
        }

        protected void gvCourses_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCourses.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}
