<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageCourses.aspx.cs" Inherits="Techspire_LMS.Admin.ManageCourses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Courses</h1>
    <asp:Literal ID="litMessage" runat="server" />
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                           HeaderText="Please fix the following:" DisplayMode="BulletList" />

    <div class="admin-panel">
        <asp:HiddenField ID="hfCourseId" runat="server" Value="0" />
        <h3><asp:Literal ID="litFormTitle" runat="server" Text="Add a course" /></h3>

        <div class="form-field">
            <label for="<%= txtTitle.ClientID %>">Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="200" />
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Title is required." />
        </div>
        <div class="form-field">
            <label for="<%= ddlCategory.ClientID %>">Category</label>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" />
        </div>
        <div class="form-field">
            <label for="<%= txtShortDescription.ClientID %>">Short description</label>
            <asp:TextBox ID="txtShortDescription" runat="server" CssClass="form-control" MaxLength="300" />
        </div>
        <div class="form-field">
            <label for="<%= txtFullDescription.ClientID %>">Full description</label>
            <asp:TextBox ID="txtFullDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
        </div>
        <div class="form-field">
            <label for="<%= ddlDifficulty.ClientID %>">Difficulty</label>
            <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="form-control">
                <asp:ListItem Text="Beginner" Value="Beginner" />
                <asp:ListItem Text="Intermediate" Value="Intermediate" />
                <asp:ListItem Text="Advanced" Value="Advanced" />
            </asp:DropDownList>
        </div>
        <div class="form-field">
            <label for="<%= txtDuration.ClientID %>">Duration (minutes)</label>
            <asp:TextBox ID="txtDuration" runat="server" CssClass="form-control" TextMode="Number" />
        </div>
        <div class="form-field">
            <label for="<%= txtThumbnail.ClientID %>">Thumbnail</label>
            <asp:Image ID="imgThumbnailPreview" runat="server" Visible="false"
                       style="max-width:160px; max-height:100px; display:block; margin-bottom:0.5rem; border-radius:var(--radius);" />
            <asp:FileUpload ID="fuThumbnail" runat="server" CssClass="form-control" />
            <span class="form-hint">JPG, PNG, GIF or WEBP, up to 2 MB. Leave empty to keep the current image.</span>
            <asp:TextBox ID="txtThumbnail" runat="server" CssClass="form-control" ReadOnly="true"
                         style="margin-top:0.4rem; background:#f3f4f7;" placeholder="No thumbnail uploaded yet" />
        </div>
        <div class="form-field">
            <asp:CheckBox ID="chkIsPublished" runat="server" Text="Published (visible to learners)" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
    </div>

    <div class="admin-panel">
        <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                      DataKeyNames="CourseID" OnRowCommand="gvCourses_RowCommand"
                      AllowPaging="true" PageSize="10" OnPageIndexChanging="gvCourses_PageIndexChanging">
            <PagerStyle CssClass="table-pager" />
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                <asp:BoundField DataField="DifficultyLevel" HeaderText="Difficulty" />
                <asp:BoundField DataField="EnrolmentCount" HeaderText="Enrolled" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("IsPublished") ? "badge" : "badge badge-draft" %>'>
                            <%# (bool)Eval("IsPublished") ? "Published" : "Draft" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("CourseID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("CourseID") %>' CssClass="btn btn-danger btn-small" CausesValidation="false"
                                        OnClientClick="return confirm('Delete this course? This also removes its lessons and quizzes.');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
