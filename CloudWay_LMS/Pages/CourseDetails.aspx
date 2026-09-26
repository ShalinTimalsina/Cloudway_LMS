<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="CourseDetails.aspx.cs" Inherits="CloudWay_LMS.Pages.CourseDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
        <div class="alert alert-error">This course could not be found.</div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phCourse" runat="server">
        <asp:Literal ID="litMessage" runat="server" />

        <!-- Breadcrumbs -->
        <div class="breadcrumb-bar" style="padding-top:1rem;">
            <a href="<%= ResolveUrl("~/Pages/Default.aspx") %>">Home</a>
            <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
            <a href="<%= ResolveUrl("~/Pages/Courses.aspx") %>">Courses</a>
            <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
            <span class="breadcrumb-current"><%= CurrentCourse != null ? CurrentCourse.Title : "" %></span>
        </div>

    <div style="display:flex; gap:3rem; flex-wrap:wrap; align-items:flex-start; margin-top:1rem;">
        <!-- Main Content -->
        <div style="flex:2; min-width:300px;">
            <div style="margin-bottom: 2rem; padding-bottom: 2rem; border-bottom: 1px solid var(--border);">
                <span class="badge" style="margin-bottom:1rem;"><asp:Literal ID="litCategory" runat="server" /></span>
                <h1 class="page-title" style="margin-bottom:0.5rem;"><asp:Literal ID="litTitle" runat="server" /></h1>
                <p class="page-subtitle" style="margin-bottom:1.5rem; display:flex; align-items:center; gap:0.5rem;">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
                    <asp:Literal ID="litMeta" runat="server" />
                </p>

                <div class="tag-list">
                    <asp:Repeater ID="rptTags" runat="server">
                        <ItemTemplate>
                            <a class="tag" href="<%# ResolveUrl("~/Pages/Courses.aspx?tag=") + Eval("TagID") %>"><%# Eval("Name") %></a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <h2 style="margin-bottom:1rem; font-size:1.5rem; display:flex; align-items:center; gap:0.5rem;">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--brand)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"></path><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"></path></svg>
                About this course
            </h2>

            <div style="font-size:16px; line-height:1.7; color:var(--text); margin-bottom:3rem; padding:1.5rem; background:var(--surface); border-radius:var(--radius); border:1px solid var(--border);">
                <asp:Literal ID="litDescription" runat="server" />
            </div>

            <h2 style="margin-bottom:1.5rem; font-size:1.5rem; display:flex; align-items:center; gap:0.5rem;">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--brand)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 2 7 12 12 22 7 12 2"></polygon><polyline points="2 17 12 22 22 17"></polyline><polyline points="2 12 12 17 22 12"></polyline></svg>
                Lessons
            </h2>
            <asp:Repeater ID="rptLessons" runat="server">
                <HeaderTemplate><div style="display:flex; flex-direction:column; gap:1rem; margin-bottom:3rem;"></HeaderTemplate>
                <ItemTemplate>
                    <a href="<%# ResolveUrl("~/Pages/LessonDetails.aspx?id=") + Eval("LessonID") %>" class="card card-clickable" style="flex-direction:row; justify-content:space-between; align-items:center; padding:1.25rem 1.5rem; text-decoration:none; color:var(--text);">
                        <span style="font-weight:500; display:flex; align-items:center; gap:0.75rem;">
                            <%# IsLessonDone(Eval("LessonID")) ? "<svg width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='var(--brand)' stroke-width='3' stroke-linecap='round' stroke-linejoin='round'><polyline points='20 6 9 17 4 12'></polyline></svg>" : "<svg width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='opacity:0.3;'><circle cx='12' cy='12' r='10'></circle></svg>" %>
                            <%# Eval("Title") %>
                        </span>
                        <span class="card-meta" style="margin:0; font-size:0.9rem;">Self-paced</span>
                    </a>
                </ItemTemplate>
                <FooterTemplate></div></FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="litNoLessons" runat="server" Visible="false"><p style="margin-bottom:3rem; opacity:0.7;">No lessons published yet.</p></asp:Literal>

            <h2 style="margin-bottom:1.5rem; font-size:1.5rem; display:flex; align-items:center; gap:0.5rem;">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--brand)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>
                Quizzes
            </h2>
            <asp:Repeater ID="rptQuizzes" runat="server">
                <HeaderTemplate><div style="display:flex; flex-direction:column; gap:1rem;"></HeaderTemplate>
                <ItemTemplate>
                    <a href="<%# ResolveUrl("~/Member/Quiz.aspx?id=") + Eval("QuizID") %>" class="card card-clickable" style="flex-direction:row; justify-content:space-between; align-items:center; padding:1.25rem 1.5rem; text-decoration:none; color:var(--text);">
                        <span style="font-weight:500; display:flex; align-items:center; gap:0.75rem;">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--accent)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line><polyline points="10 9 9 9 8 9"></polyline></svg>
                            <%# Eval("Title") %>
                        </span>
                        <span class="card-meta" style="margin:0; font-size:0.9rem;"><%# Eval("QuestionCount") %> questions &bull; Pass: <%# Eval("PassingScore") %>%</span>
                    </a>
                </ItemTemplate>
                <FooterTemplate></div></FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="litNoQuizzes" runat="server" Visible="false"><p style="opacity:0.7;">No quizzes for this course yet.</p></asp:Literal>
        </div>

        <!-- Sidebar -->
        <div style="flex:1; min-width:280px;">
            <div class="card">
                <img class="card-thumb" src="<%= ResolveUrl(CurrentCourse != null ? CurrentCourse.ThumbnailOrDefault : "") %>" alt="Course Thumbnail" />
                <div style="padding:2rem;">
                <h3 style="margin-top:0; margin-bottom:1.5rem; font-size:1.25rem;">Course Enrollment</h3>
                <asp:PlaceHolder ID="phGuestPrompt" runat="server" Visible="false">
                    <p style="margin-bottom:1.5rem; color:var(--text); opacity:0.85; line-height:1.5;">You must be logged in to enroll in this course and track your progress.</p>
                    <a class="btn" style="width:100%; text-align:center;" href="<%= ResolveUrl("~/Account/Login.aspx") %>">Log in to enroll</a>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phEnrolAction" runat="server" Visible="false">
                    <p style="margin-bottom:1.5rem; color:var(--text); opacity:0.85; line-height:1.5;">Start learning today and unlock all lessons and quizzes.</p>
                    <asp:Button ID="btnEnrol" runat="server" Text="Enroll now" CssClass="btn" style="width:100%; font-size:1.1rem; padding:0.75rem;" OnClick="btnEnrol_Click" />
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phEnrolled" runat="server" Visible="false">
                    <div style="padding:1.5rem; background:rgba(19,168,158,0.08); border-radius:var(--radius); border:1px solid rgba(19,168,158,0.2); text-align:center;">
                        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="var(--brand)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="margin-bottom:0.5rem;"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>
                        <p style="margin:0 0 0.5rem 0; font-weight:600; color:var(--brand); font-size:1.1rem;">You're enrolled!</p>
                        <p style="margin:0; font-size:14px; opacity:0.9;">Progress: <strong><asp:Literal ID="litProgress" runat="server" />%</strong></p>
                    </div>
                </asp:PlaceHolder>
                </div>
            </div>

            <!-- Course Feedback Card -->
            <div class="card" style="margin-top:1.5rem; padding:2rem; text-align:center; border-top: 3px solid var(--accent);">
                <div style="background: rgba(43, 209, 197, 0.1); width: 48px; height: 48px; border-radius: 50%; display:flex; align-items:center; justify-content:center; color: var(--accent); margin: 0 auto 1rem;">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path></svg>
                </div>
                <h3 style="margin:0 0 0.75rem 0; font-size:1.1rem; color:var(--text);">Course Feedback</h3>
                <p style="font-size:14px; opacity:0.8; margin-bottom:1.5rem; line-height: 1.5;">Have thoughts on this course? Help us improve by leaving a review.</p>
                <a href="<%= ResolveUrl("~/Pages/Contact.aspx?subject=Course Feedback: ") + (CurrentCourse != null ? Server.UrlEncode(CurrentCourse.Title) : "") %>" class="btn btn-outline" style="width:100%; border-color: var(--accent); color: var(--accent);">Leave Feedback</a>
            </div>

        </div>
    </div>
    </asp:PlaceHolder>
</asp:Content>
