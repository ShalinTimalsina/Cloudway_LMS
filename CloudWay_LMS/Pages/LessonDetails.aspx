<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="LessonDetails.aspx.cs" Inherits="CloudWay_LMS.Pages.LessonDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
        <div class="alert alert-error">This lesson could not be found.</div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phLesson" runat="server">
        <div style="max-width: 900px; margin: 0 auto; padding-bottom: 4rem;">
            <!-- Breadcrumbs -->
            <div class="breadcrumb-bar" style="padding-top:1rem;">
                <a href="<%= ResolveUrl("~/Pages/Default.aspx") %>">Home</a>
                <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
                <a href="<%= ResolveUrl("~/Pages/Courses.aspx") %>">Courses</a>
                <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
                <a href="<%= ResolveUrl("~/Pages/CourseDetails.aspx?id=" + CourseId) %>"><asp:Literal ID="litCourseTitle" runat="server" /></a>
                <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
                <span class="breadcrumb-current"><%= BreadcrumbTitle %></span>
            </div>

            <h1 class="page-title"><asp:Literal ID="litTitle" runat="server" /></h1>
            <p class="page-subtitle"><asp:Literal ID="litMeta" runat="server" /></p>

            <asp:Literal ID="litMessage" runat="server" />

            <asp:PlaceHolder ID="phEnrolPrompt" runat="server" Visible="false">
                <div class="alert alert-info" style="margin-top:2rem;">
                    <h3 style="margin-top:0;">You are not enrolled</h3>
                    <p>Enrol in this course to view the lesson content, watch videos, and track your progress.</p>
                    <a class="btn btn-outline" href="<%= ResolveUrl("~/Pages/CourseDetails.aspx?id=" + CourseId) %>" style="margin-top:1rem; display:inline-block;">Go to Course Page</a>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phVideo" runat="server" Visible="false">
                <div style="margin: 2rem 0; border-radius: var(--radius); overflow: hidden; box-shadow: var(--shadow); background: black;">
                    <asp:PlaceHolder ID="phVideoFile" runat="server" Visible="false">
                        <video controls style="width:100%; max-width:100%; display:block;">
                            <source src="<%= VideoUrl %>" />
                            Your browser doesn't support embedded video.
                            <a href="<%= VideoUrl %>">Download the video instead</a>.
                        </video>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phVideoEmbed" runat="server" Visible="false">
                        <div style="position:relative; padding-bottom:56.25%; height:0; width:100%;">
                            <iframe src="<%= VideoUrl %>" title="Lesson video" frameborder="0" allowfullscreen
                                    style="position:absolute; top:0; left:0; width:100%; height:100%; border:none;"></iframe>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phVideoLink" runat="server" Visible="false">
                        <div style="padding: 3rem; text-align: center; background: white;">
                            <p><a class="btn" target="_blank" rel="noopener" href="<%= VideoUrl %>">Watch the lesson video &rarr;</a></p>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </asp:PlaceHolder>

            <%-- ContentHtml is intentionally rendered unescaped --%>
            <asp:Literal ID="litContent" runat="server" />

            <asp:Repeater ID="rptResources" runat="server">
                <HeaderTemplate>
                    <div class="form-card" style="max-width: none; margin-top: 2rem; background: #f8fafc; padding: 1.5rem 2rem;">
                        <h3 style="margin-top:0; color: var(--brand);">Resources & Downloads</h3>
                        <ul style="margin-bottom:0; padding-left: 1.5rem;">
                </HeaderTemplate>
                <ItemTemplate>
                    <li style="margin-bottom: 0.5rem;">
                        <a href='<%# ResolveUrl((string)Eval("FilePath")) %>' target="_blank" rel="noopener" style="font-weight: 500;"><%# Eval("FileName") %></a>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                        </ul>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="litNoResources" runat="server" Visible="false">
                <div class="form-card" style="max-width: none; margin-top: 2rem; background: #f8fafc; padding: 1.5rem 2rem;">
                    <h3 style="margin-top:0; color: var(--brand);">Resources & Downloads</h3>
                    <p style="margin-bottom:0; color: var(--text-muted);">No resources attached to this lesson.</p>
                </div>
            </asp:Literal>

            <div style="display:flex; justify-content:space-between; align-items:center; background: white; padding: 1.5rem; border-radius: var(--radius); box-shadow: var(--shadow); margin-top:3rem; border: 1px solid var(--border);">
                <div style="flex:1;">
                    <asp:HyperLink ID="lnkPrev" runat="server" CssClass="btn btn-outline" Visible="false">&larr; Previous</asp:HyperLink>
                </div>
                
                <div style="flex:1; text-align: center;">
                    <asp:PlaceHolder ID="phCompleted" runat="server" Visible="false">
                        <span class="badge" style="background: var(--success); color: white; padding: 0.5rem 1rem; font-size: 1rem; border-radius: 99px;">&#10003; Completed</span>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phMarkComplete" runat="server" Visible="false">
                        <asp:Button ID="btnMarkComplete" runat="server" Text="Mark as Complete" CssClass="btn" style="background: var(--success); border-color: var(--success); color: white;" OnClick="btnMarkComplete_Click" />
                    </asp:PlaceHolder>
                </div>
                
                <div style="flex:1; text-align: right;">
                    <asp:HyperLink ID="lnkNext" runat="server" CssClass="btn btn-outline" Visible="false">Next &rarr;</asp:HyperLink>
                    <% if (!lnkNext.Visible) { %>
                        <a href="<%= ResolveUrl("~/Pages/CourseDetails.aspx?id=" + CourseId) %>" class="btn">Course Overview &rarr;</a>
                    <% } %>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
