<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="CourseDetails.aspx.cs" Inherits="Techspire_LMS.Pages.CourseDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
        <div class="alert alert-error">This course could not be found.</div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phCourse" runat="server">
        <asp:Literal ID="litMessage" runat="server" />

        <span class="badge"><asp:Literal ID="litCategory" runat="server" /></span>
        <h1 class="page-title"><asp:Literal ID="litTitle" runat="server" /></h1>
        <p class="page-subtitle">
            <asp:Literal ID="litMeta" runat="server" />
        </p>

        <div class="tag-list" style="margin-bottom:1rem;">
            <asp:Repeater ID="rptTags" runat="server">
                <ItemTemplate>
                    <a class="tag" href="<%# ResolveUrl("~/Pages/Courses.aspx?tag=") + Eval("TagID") %>"><%# Eval("TagName") %></a>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <p><asp:Literal ID="litDescription" runat="server" /></p>

        <div class="admin-panel" style="max-width:420px;">
            <asp:PlaceHolder ID="phGuestPrompt" runat="server" Visible="false">
                <p>You must be logged in to enrol.</p>
                <a class="btn" href="<%= ResolveUrl("~/Account/Login.aspx") %>">Log in to enrol</a>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEnrolAction" runat="server" Visible="false">
                <asp:Button ID="btnEnrol" runat="server" Text="Enrol in this course" CssClass="btn" OnClick="btnEnrol_Click" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEnrolled" runat="server" Visible="false">
                <p><strong>You're enrolled.</strong> Progress: <asp:Literal ID="litProgress" runat="server" />%</p>
            </asp:PlaceHolder>
        </div>

        <h2>Lessons</h2>
        <asp:Repeater ID="rptLessons" runat="server">
            <HeaderTemplate><ol></HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href="<%# ResolveUrl("~/Pages/LessonDetails.aspx?id=") + Eval("LessonID") %>">
                        <%# IsLessonDone(Eval("LessonID")) ? "&#10003; " : "" %><%# Eval("Title") %>
                    </a>
                    <span class="card-meta">
                        <%# Eval("DurationMinutes") != null ? Eval("DurationMinutes") + " min" : "" %>
                    </span>
                </li>
            </ItemTemplate>
            <FooterTemplate></ol></FooterTemplate>
        </asp:Repeater>
        <asp:Literal ID="litNoLessons" runat="server" Visible="false"><p>No lessons published yet.</p></asp:Literal>

        <h2>Quizzes</h2>
        <asp:Repeater ID="rptQuizzes" runat="server">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href="<%# ResolveUrl("~/Member/Quiz.aspx?id=") + Eval("QuizID") %>"><%# Eval("Title") %></a>
                    <span class="card-meta">(<%# Eval("QuestionCount") %> questions, pass mark <%# Eval("PassMark") %>%)</span>
                </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <asp:Literal ID="litNoQuizzes" runat="server" Visible="false"><p>No quizzes for this course yet.</p></asp:Literal>
    </asp:PlaceHolder>
</asp:Content>
