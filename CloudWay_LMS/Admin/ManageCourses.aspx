<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageCourses.aspx.cs" Inherits="CloudWay_LMS.Admin.ManageCourses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 2rem;">
        <h1 class="page-title" style="margin:0;">Manage Courses</h1>
    </div>

    <asp:Literal ID="litMessage" runat="server" />
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                           HeaderText="Please fix the following:" DisplayMode="BulletList" />

    <style>
        .courses-layout {
            display: grid;
            grid-template-columns: 1fr;
            gap: 2rem;
            align-items: start;
        }
        @media (min-width: 992px) {
            .courses-layout {
                grid-template-columns: 400px 1fr;
            }
            .form-sticky {
                position: sticky;
                top: 2rem;
            }
        }
    </style>

    <div class="courses-layout">
        <!-- Sidebar Form -->
        <div class="card form-sticky" style="padding: 2rem;">
            <asp:HiddenField ID="hfCourseId" runat="server" Value="0" />
            <h3 style="margin-top: 0; margin-bottom: 1.5rem; font-family: var(--font-heading); font-size: 1.25rem; font-weight: 600;">
                <asp:Literal ID="litFormTitle" runat="server" Text="Add New Course" />
            </h3>

            <div class="form-field">
                <label for="<%= txtTitle.ClientID %>" style="font-size: 14px; color: var(--text-muted); margin-bottom: 0.5rem; display: block;">Course Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="200" placeholder="e.g. Introduction to Programming" />
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                    CssClass="field-error" Display="Dynamic" ErrorMessage="Title is required." />
            </div>
            <div class="form-field">
                <label for="<%= ddlCategory.ClientID %>" style="font-size: 14px; color: var(--text-muted); margin-bottom: 0.5rem; display: block;">Category</label>
                <div class="custom-select-wrapper">
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="form-field">
                <label for="<%= txtDescription.ClientID %>" style="font-size: 14px; color: var(--text-muted); margin-bottom: 0.5rem; display: block;">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Briefly describe the course..." style="height: auto; padding-top: 10px;" />
            </div>
            <div class="form-field">
                <label for="<%= fuThumbnail.ClientID %>" style="font-size: 14px; color: var(--text-muted); margin-bottom: 0.5rem; display: block;">Thumbnail</label>
                <asp:Image ID="imgThumbnailPreview" runat="server" Visible="false"
                           style="width: 100%; height: 140px; object-fit: cover; display:block; margin-bottom:0.75rem; border-radius:var(--radius); border: 1px solid var(--border);" />
                <asp:FileUpload ID="fuThumbnail" runat="server" CssClass="form-control" style="padding-top: 6px;" />
                <span class="form-hint" style="display: block; margin-top: 0.5rem; color: var(--text-muted); font-size: 12px;">JPG, PNG, GIF or WEBP, up to 2 MB. Leave empty to keep the current image.</span>
                <asp:TextBox ID="txtThumbnail" runat="server" CssClass="form-control" ReadOnly="true"
                             style="margin-top:0.5rem; background: var(--bg-color); font-size: 13px;" placeholder="No thumbnail uploaded yet" />
            </div>
            <div class="form-field" style="display: flex; align-items: center; gap: 0.5rem;">
                <asp:CheckBox ID="chkIsPublished" runat="server" />
                <label for="<%= chkIsPublished.ClientID %>" style="margin: 0; font-weight: 500;">Published (visible to learners)</label>
            </div>

            <div style="display: flex; gap: 0.75rem; margin-top: 2rem;">
                <asp:Button ID="btnSave" runat="server" Text="Save Course" CssClass="btn" OnClick="btnSave_Click" style="flex: 1;" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" style="flex: 1;" />
            </div>
        </div>

        <!-- Main Content -->
        <div class="card" style="padding: 2rem; overflow-x: auto;">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 1.5rem;">
                <h3 style="margin: 0; font-family: var(--font-heading); font-size: 1.25rem; font-weight: 600;">Course Directory</h3>
            </div>
            <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                          DataKeyNames="CourseID" OnRowCommand="gvCourses_RowCommand"
                          AllowPaging="true" PageSize="10" OnPageIndexChanging="gvCourses_PageIndexChanging" GridLines="None">
                <PagerStyle CssClass="table-pager" />
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Title" ItemStyle-Font-Bold="true" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Category" ItemStyle-CssClass="text-muted" />
                    <asp:BoundField DataField="EnrollmentCount" HeaderText="Enrolled" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    
                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsPublished") ? "badge badge-success" : "badge badge-danger" %>' style="font-size: 12px; padding: 4px 10px; border-radius: 99px;">
                                <%# (bool)Eval("IsPublished") ? "Published" : "Draft" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <div style="display:flex; justify-content:flex-end; gap:0.5rem;">
                                <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("CourseID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("CourseID") %>' CssClass="btn btn-danger btn-small" CausesValidation="false"
                                                OnClientClick="return confirm('Delete this course? This also removes its lessons and quizzes.');">Delete</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
