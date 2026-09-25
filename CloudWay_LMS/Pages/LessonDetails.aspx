<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="LessonDetails.aspx.cs" Inherits="CloudWay_LMS.Pages.LessonDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
        <div class="alert alert-error">This lesson could not be found.</div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phLesson" runat="server">
        <p class="card-meta">
            <a href="<%= ResolveUrl("~/Pages/CourseDetails.aspx?id=" + CourseId) %>">
                &larr; <asp:Literal ID="litCourseTitle" runat="server" />
            </a>
        </p>

        <h1 class="page-title"><asp:Literal ID="litTitle" runat="server" /></h1>
        <p class="page-subtitle"><asp:Literal ID="litMeta" runat="server" /></p>

        <asp:Literal ID="litMessage" runat="server" />

        <div class="admin-panel" style="max-width:600px;">
            <asp:PlaceHolder ID="phCompleted" runat="server" Visible="false">
                <span class="badge">&#10003; Completed</span>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phMarkComplete" runat="server" Visible="false">
                <asp:Button ID="btnMarkComplete" runat="server" Text="Mark this lesson complete" CssClass="btn" OnClick="btnMarkComplete_Click" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phEnrolPrompt" runat="server" Visible="false">
                <p style="margin:0;">Enrol in this course to track your progress through it.</p>
            </asp:PlaceHolder>
        </div>

        <%-- ContentHtml is intentionally rendered unescaped — lessons are
             authored only by Admins through ManageLessons.aspx, which sits
             behind Admin.master's enforcement. If lesson authoring is ever
             opened up to a lower-trust role, this becomes a stored-XSS
             vector and would need sanitizing (e.g. HtmlSanitizer) before
             going out this way. --%>
        <div class="form-card" style="max-width:none;">
            <asp:Literal ID="litContent" runat="server" />
        </div>

        <asp:PlaceHolder ID="phVideo" runat="server" Visible="false">
            <h2>Video</h2>

            <%-- Real <video>/<iframe> embedding (F4 — multimedia), not just a
                 link out. Which markup renders depends on what VideoUrl looks
                 like — decided once in code-behind (see DetectVideoKind) so
                 the markup itself stays a simple three-way switch instead of
                 parsing URLs inline. --%>
            <asp:PlaceHolder ID="phVideoFile" runat="server" Visible="false">
                <video controls style="width:100%; max-width:720px; border-radius:var(--radius);">
                    <source src="<%= VideoUrl %>" />
                    Your browser doesn't support embedded video.
                    <a href="<%= VideoUrl %>">Download the video instead</a>.
                </video>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phVideoEmbed" runat="server" Visible="false">
                <div style="position:relative; padding-bottom:56.25%; height:0; max-width:720px;">
                    <iframe src="<%= VideoUrl %>" title="Lesson video" frameborder="0" allowfullscreen
                            style="position:absolute; top:0; left:0; width:100%; height:100%; border-radius:var(--radius);"></iframe>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phVideoLink" runat="server" Visible="false">
                <p><a class="btn btn-outline" target="_blank" rel="noopener" href="<%= VideoUrl %>">Watch the lesson video &rarr;</a></p>
            </asp:PlaceHolder>
        </asp:PlaceHolder>

        <h2>Resources</h2>
        <asp:Repeater ID="rptResources" runat="server">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href='<%# ResolveUrl((string)Eval("FilePath")) %>' target="_blank" rel="noopener"><%# Eval("Title") %></a>
                    <span class="card-meta">(<%# Eval("ResourceType") %>)</span>
                </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <asp:Literal ID="litNoResources" runat="server" Visible="false"><p>No resources attached to this lesson.</p></asp:Literal>

        <div style="display:flex; justify-content:space-between; margin-top:2rem;">
            <asp:HyperLink ID="lnkPrev" runat="server" CssClass="btn btn-outline" Visible="false">&larr; Previous lesson</asp:HyperLink>
            <asp:HyperLink ID="lnkNext" runat="server" CssClass="btn btn-outline" Visible="false">Next lesson &rarr;</asp:HyperLink>
        </div>
    </asp:PlaceHolder>
</asp:Content>
