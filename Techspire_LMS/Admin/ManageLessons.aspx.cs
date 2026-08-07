using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Techspire_LMS.BLL;
using Techspire_LMS.Models;

namespace Techspire_LMS.Admin
{
    public partial class ManageLesson : System.Web.UI.Page
    {
        private readonly LessonBLL _bll = new LessonBLL();
        private readonly ResourceBLL _resourceBll = new ResourceBLL();

        private static readonly string[] AllowedResourceExtensions =
        {
            ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx",
            ".zip", ".txt", ".md", ".csv", ".json",
            ".png", ".jpg", ".jpeg", ".gif",
            ".py", ".js", ".cs", ".html", ".css", ".sql"
        };
        private const int MaxResourceBytes = 10 * 1024 * 1024; // 10 MB
        private const string ResourceUploadFolder = "~/Content/uploads/resources/";

        private int SelectedCourseId
        {
            get { return string.IsNullOrEmpty(ddlCourse.SelectedValue) ? 0 : int.Parse(ddlCourse.SelectedValue); }
        }
        private int CurrentLessonId
        {
            get { return int.Parse(hfLessonId.Value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourseDropdown();
                if (SelectedCourseId > 0) BindGrid();
            }
        }

        private void BindCourseDropdown()
        {
            ddlCourse.Items.Clear();
            ddlCourse.Items.Add(new ListItem("— Select a course —", ""));
            foreach (var c in new CourseBLL().GetAllForAdmin())
                ddlCourse.Items.Add(new ListItem(c.Title, c.CourseID.ToString()));
        }

        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            phCourseTools.Visible = SelectedCourseId > 0;
            ResetForm();
            if (SelectedCourseId > 0) BindGrid();
        }

        private void BindGrid()
        {
            phCourseTools.Visible = true;
            gvLessons.DataSource = _bll.GetByCourse(SelectedCourseId);
            gvLessons.DataBind();
        }

        // ====================================================================
        // Lesson CRUD
        // ====================================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Lesson l = new Lesson
                {
                    LessonID = CurrentLessonId,
                    CourseID = SelectedCourseId,
                    Title = txtTitle.Text,
                    ContentHtml = txtContent.Text,
                    VideoUrl = txtVideoUrl.Text,
                    SortOrder = string.IsNullOrWhiteSpace(txtSortOrder.Text) ? 1 : int.Parse(txtSortOrder.Text),
                    DurationMinutes = string.IsNullOrWhiteSpace(txtDuration.Text) ? (int?)null : int.Parse(txtDuration.Text)
                };

                int savedLessonId;
                if (l.LessonID == 0)
                {
                    savedLessonId = _bll.Add(l);
                    litMessage.Text = Success("Lesson added.");
                }
                else
                {
                    _bll.Edit(l);
                    savedLessonId = l.LessonID;
                    litMessage.Text = Success("Lesson updated.");
                }

                BindGrid();

                // Drop straight into "editing" that same lesson so the admin
                // can immediately attach resources without a second click —
                // saving a brand-new lesson is exactly when you have files
                // ready to attach to it.
                LoadLessonIntoForm(savedLessonId);
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }
        }

        protected void gvLessons_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int lessonId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                LoadLessonIntoForm(lessonId);
            }
            else if (e.CommandName == "DeleteRow")
            {
                try
                {
                    _bll.Remove(lessonId); // Resources cascade — see CreateDatabase.sql
                    litMessage.Text = Success("Lesson deleted.");
                }
                catch (ValidationException vex)
                {
                    litMessage.Text = Error(vex.Message);
                }

                if (CurrentLessonId == lessonId) ResetForm();
                BindGrid();
            }
        }

        private void LoadLessonIntoForm(int lessonId)
        {
            Lesson l = _bll.GetById(lessonId);
            if (l == null) return;

            hfLessonId.Value = l.LessonID.ToString();
            txtTitle.Text = l.Title;
            txtContent.Text = l.ContentHtml;
            txtVideoUrl.Text = l.VideoUrl;
            txtSortOrder.Text = l.SortOrder.ToString();
            txtDuration.Text = l.DurationMinutes.HasValue ? l.DurationMinutes.Value.ToString() : "";
            litFormTitle.Text = "Edit lesson";

            litCurrentLessonTitle.Text = Server.HtmlEncode(l.Title);
            phResources.Visible = true;
            BindResourcesGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            hfLessonId.Value = "0";
            txtTitle.Text = "";
            txtContent.Text = "";
            txtVideoUrl.Text = "";
            txtSortOrder.Text = "1";
            txtDuration.Text = "";
            litFormTitle.Text = "Add a lesson";
            phResources.Visible = false;
        }

        // ====================================================================
        // Resource upload — only reachable once a lesson has a real LessonID
        // ====================================================================
        private void BindResourcesGrid()
        {
            var resources = _resourceBll.GetByLesson(CurrentLessonId);
            gvResources.DataSource = resources;
            gvResources.DataBind();
            litNoResources.Visible = resources.Count == 0;
        }

        protected void btnUploadResource_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentLessonId == 0)
                    throw new ValidationException("Save the lesson before attaching resources to it.");

                if (!fuResource.HasFile)
                    throw new ValidationException("Choose a file to upload.");

                string extension = Path.GetExtension(fuResource.FileName).ToLowerInvariant();
                if (Array.IndexOf(AllowedResourceExtensions, extension) < 0)
                    throw new ValidationException("That file type isn't allowed.");

                if (fuResource.PostedFile.ContentLength > MaxResourceBytes)
                    throw new ValidationException("File must be 10 MB or smaller.");

                string folderPath = Server.MapPath(ResourceUploadFolder);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                // Random filename on disk, original name kept only in Title —
                // same path-traversal/collision reasoning as the course
                // thumbnail upload (see ManageCourses.aspx.cs).
                string safeFileName = Guid.NewGuid().ToString("N") + extension;
                fuResource.SaveAs(Path.Combine(folderPath, safeFileName));

                Resource r = new Resource
                {
                    LessonID = CurrentLessonId,
                    Title = string.IsNullOrWhiteSpace(txtResourceTitle.Text)
                        ? fuResource.FileName : txtResourceTitle.Text,
                    FilePath = ResourceUploadFolder + safeFileName,
                    ResourceType = ddlResourceType.SelectedValue
                };
                _resourceBll.Add(r);

                litMessage.Text = Success("Resource uploaded.");
                txtResourceTitle.Text = "";
                BindResourcesGrid();
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }
        }

        protected void gvResources_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteRow") return;

            int resourceId = int.Parse((string)e.CommandArgument);
            try
            {
                _resourceBll.Remove(resourceId);
                litMessage.Text = Success("Resource deleted.");
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }

            BindResourcesGrid();
        }

        private string Success(string msg) { return "<div class=\"alert alert-success\">" + Server.HtmlEncode(msg) + "</div>"; }
        private string Error(string msg) { return "<div class=\"alert alert-error\">" + Server.HtmlEncode(msg) + "</div>"; }
    }
}