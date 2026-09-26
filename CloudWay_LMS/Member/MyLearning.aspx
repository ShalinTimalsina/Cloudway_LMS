<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Masterpages/Site.master" CodeBehind="MyLearning.aspx.cs" Inherits="CloudWay_LMS.Member.MyLearning" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Breadcrumbs -->
    <div class="breadcrumb-bar">
        <a href="<%= ResolveUrl("~/Pages/Default.aspx") %>">Home</a>
        <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
        <span class="breadcrumb-current">My Learning</span>
    </div>

    <h1 class="page-title">My Learning</h1>
    <h2>My Courses</h2>
    <asp:Repeater ID="rptEnrollments" runat="server" OnItemCommand="rptEnrollments_ItemCommand">
        <HeaderTemplate><div class="card-grid"></HeaderTemplate>
        <ItemTemplate>
            <div class="card card-clickable" onclick="window.location.href='<%# ResolveUrl("~/Pages/CourseDetails.aspx?id=") + Eval("CourseID") %>';">
                <img class="card-thumb" src="<%# ResolveUrl(ThumbnailOrDefault((string)Eval("ThumbnailPath"))) %>" alt="" />
                <div class="card-body">
                    <h3 class="card-title"><%# Eval("CourseTitle") %></h3>
                    <p class="card-meta">
                        <%# ((bool)Eval("IsCompleted")) ? "Completed" : "In progress" %>
                        - <%# Eval("ProgressPercent", "{0:0}") %>%
                    </p>
                    <div style="background:var(--border); border-radius:999px; height:8px; overflow:hidden;">
                        <div style='background:var(--accent); height:100%; width:<%# Eval("ProgressPercent", "{0:0}") %>%;'></div>
                    </div>
                    <a class="btn btn-outline btn-small" style="margin-top:0.5rem;"
                       href="<%# ResolveUrl("~/Pages/CourseDetails.aspx?id=") + Eval("CourseID") %>">Continue</a>
                    <a class="btn btn-outline btn-small" style="margin-top:0.5rem; border-color: var(--accent); color: var(--accent);" onclick="event.stopPropagation();"
                       href="<%# ResolveUrl("~/Pages/Contact.aspx?subject=Course Feedback: ") + Server.UrlEncode(Eval("CourseTitle").ToString()) %>">Feedback</a>
                    <asp:LinkButton runat="server" CommandName="Unenroll" CommandArgument='<%# Eval("EnrollmentID") %>'
                                    CssClass="btn btn-danger btn-small" style="margin-top:0.5rem;"
                                    OnClientClick="return confirm('Unenroll from this course? Your lesson progress will be reset; your quiz history is kept.');">Unenroll</asp:LinkButton>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
   <asp:Panel ID="litNoEnrollments" runat="server" Visible="false">
    <div style="text-align:center; padding: 4rem 2rem; background: var(--surface); border-radius: var(--radius); border: 1px dashed var(--border); margin-bottom: 2rem;">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--brand)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="margin-bottom: 1rem; opacity: 0.8;"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>
        <h3 style="margin-top: 0; margin-bottom: 0.5rem; font-size: 1.25rem;">No Courses Yet</h3>
        <p style="color: var(--text); opacity: 0.7; margin-bottom: 1.5rem;">You haven't enrolled in any courses yet. Start your learning journey today!</p>
        <a href="<%= ResolveUrl("~/Pages/Courses.aspx") %>" class="btn" style="display: inline-flex; align-items: center; gap: 0.5rem; padding: 0.75rem 1.5rem; font-weight: 500;">
            Browse Courses
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>
        </a>
    </div>
</asp:Panel>

    <h2 style="margin-top:2rem;">Quiz History</h2>
    <div class="card" style="padding: 2rem; margin-bottom: 2rem;">
        <asp:GridView ID="gvAttempts" runat="server" AutoGenerateColumns="false" CssClass="table-admin">
            <Columns>
                <asp:TemplateField HeaderText="Quiz">
                    <ItemTemplate>
                        <a href="<%# ResolveUrl("~/Member/Quiz.aspx?id=") + Eval("QuizID") %>" style="font-weight: 500;"><%# Eval("QuizTitle") %></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Score">
                    <ItemTemplate><%# Eval("Score") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Result">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("IsPassed") ? "badge badge-success" : "badge badge-danger" %>'>
                            <%# (bool)Eval("IsPassed") ? "Passed" : "Not passed" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AttemptedAt" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>
        <asp:Panel ID="litNoAttempts" runat="server" Visible="false">
            <div style="text-align:center; padding: 3rem 2rem;">
                <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="margin-bottom: 1rem; opacity: 0.4;"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>
                <h4 style="margin-top: 0; margin-bottom: 0.5rem; font-size: 1.1rem; color: var(--text);">No Quiz Attempts</h4>
                <p style="color: var(--text); opacity: 0.6; margin: 0;">You haven't taken any quizzes yet.</p>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

