<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageLessons.aspx.cs" Inherits="CloudWay_LMS.Admin.ManageLesson" %>


<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Lessons</h1>
    <asp:Literal ID="litMessage" runat="server" />

    <div class="admin-panel">
        <div class="form-field" style="max-width:400px;">
            <label for="<%= ddlCourse.ClientID %>">Course</label>
            <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" CausesValidation="false"
                              AutoPostBack="true" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged" />
        </div>
    </div>

    <asp:PlaceHolder ID="phCourseTools" runat="server" Visible="false">
        <div class="admin-panel">
            <asp:HiddenField ID="hfLessonId" runat="server" Value="0" />
            <h3><asp:Literal ID="litFormTitle" runat="server" Text="Add a lesson" /></h3>
            <asp:ValidationSummary ID="valLessonSummary" runat="server" ValidationGroup="LessonForm"
                                   CssClass="validation-summary" HeaderText="Please fix the following:" DisplayMode="BulletList" />

            <div class="form-field">
                <label for="<%= txtTitle.ClientID %>">Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="200" ValidationGroup="LessonForm" />
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                    ValidationGroup="LessonForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Lesson title is required." />
            </div>
            <div class="form-field">
                <label for="<%= txtContent.ClientID %>">Content (HTML)</label>
                <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
            </div>
            <div class="form-field">
                <label for="<%= txtVideoUrl.ClientID %>">Video URL (optional)</label>
                <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-control" />
                <span class="form-hint">For an embedded player: a direct .mp4/.webm file, a YouTube
                    <em>embed</em> link (youtube.com/embed/VIDEO_ID - not a regular watch/share link),
                    or a Vimeo player link (player.vimeo.com/video/ID). Anything else shows as a plain
                    "watch" link instead.</span>
            </div>
            <div class="form-field">
                <label for="<%= txtOrderIndex.ClientID %>">Sort order</label>
                <asp:TextBox ID="txtOrderIndex" runat="server" CssClass="form-control" TextMode="Number" Text="1" />
            </div>

            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn" ValidationGroup="LessonForm" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
        </div>

        <asp:PlaceHolder ID="phResources" runat="server" Visible="false">
            <div class="admin-panel">
                <h3>Resources for "<asp:Literal ID="litCurrentLessonTitle" runat="server" />"</h3>
                <p class="form-hint">Code samples, slide decks, cheat sheets, datasets - anything a learner
                    should be able to download for this lesson. Save the lesson itself first; resources attach
                    to an existing lesson.</p>

                <div class="form-field">
                    <label for="<%= txtResourceTitle.ClientID %>">Title</label>
                    <asp:TextBox ID="txtResourceTitle" runat="server" CssClass="form-control" MaxLength="150" />
                </div>
                <div class="form-field" style="max-width:200px;">
                    <label for="<%= ddlResourceType.ClientID %>">Type</label>
                    <asp:DropDownList ID="ddlResourceType" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Document" Value="Document" />
                        <asp:ListItem Text="Code" Value="Code" />
                        <asp:ListItem Text="Slides" Value="Slides" />
                        <asp:ListItem Text="Dataset" Value="Dataset" />
                        <asp:ListItem Text="Other" Value="Other" />
                    </asp:DropDownList>
                </div>
                <div class="form-field">
                    <label for="<%= fuResource.ClientID %>">File</label>
                    <asp:FileUpload ID="fuResource" runat="server" CssClass="form-control" />
                    <span class="form-hint">Up to 10 MB - documents, images, archives, or common code/text files.</span>
                </div>
                <asp:Button ID="btnUploadResource" runat="server" Text="Upload Resource" CssClass="btn"
                            ValidationGroup="ResourceForm" OnClick="btnUploadResource_Click" />

                <asp:GridView ID="gvResources" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                              DataKeyNames="ResourceID" OnRowCommand="gvResources_RowCommand" style="margin-top:1rem;">
                    <Columns>
                        <asp:BoundField DataField="FileName" HeaderText="Title" />
                        <asp:BoundField DataField="ResourceType" HeaderText="Type" />
                        
                        <asp:TemplateField HeaderText="File">
                            <ItemTemplate><a href='<%# ResolveUrl((string)Eval("FilePath")) %>' target="_blank" rel="noopener">Download</a></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("ResourceID") %>' CssClass="btn btn-danger btn-small"
                                                CausesValidation="false" OnClientClick="return confirm('Delete this resource?');">Delete</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Literal ID="litNoResources" runat="server" Visible="false"><p>No resources uploaded for this lesson yet.</p></asp:Literal>
            </div>
        </asp:PlaceHolder>

        <div class="admin-panel">
            <asp:GridView ID="gvLessons" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                          DataKeyNames="LessonID" OnRowCommand="gvLessons_RowCommand">
                <Columns>
                    <asp:BoundField DataField="OrderIndex" HeaderText="#" />
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("LessonID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("LessonID") %>' CssClass="btn btn-danger btn-small"
                                            CausesValidation="false" OnClientClick="return confirm('Delete this lesson?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:PlaceHolder>
</asp:Content>

