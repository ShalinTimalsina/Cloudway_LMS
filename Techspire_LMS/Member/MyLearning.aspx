<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Masterpages/Site.master" CodeBehind="MyLearning.aspx.cs" Inherits="Techspire_LMS.Member.MyLearning" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="page-title">My Learning</h1>

    <h2>My Courses</h2>
    <asp:Repeater ID="rptEnrollments" runat="server" OnItemCommand="rptEnrollments_ItemCommand">
        <HeaderTemplate><div class="card-grid"></HeaderTemplate>
        <ItemTemplate>
            <div class="card">
                <img class="card-thumb" src="<%# ResolveUrl(ThumbnailOrDefault((string)Eval("ThumbnailPath"))) %>" alt="" />
                <div class="card-body">
                    <h3 class="card-title"><%# Eval("CourseTitle") %></h3>
                    <p class="card-meta">
                        <%# ((bool)Eval("IsCompleted")) ? "Completed" : "In progress" %>
                        — <%# Eval("ProgressPercent", "{0:0}") %>%
                    </p>
                    <div style="background:var(--border); border-radius:999px; height:8px; overflow:hidden;">
                        <div style='background:var(--accent); height:100%; width:<%# Eval("ProgressPercent", "{0:0}") %>%;'></div>
                    </div>
                    <a class="btn btn-outline btn-small" style="margin-top:0.5rem;"
                       href="<%# ResolveUrl("~/Pages/CourseDetails.aspx?id=") + Eval("CourseID") %>">Continue</a>
                    <asp:LinkButton runat="server" CommandName="Unenroll" CommandArgument='<%# Eval("EnrollmentID") %>'
                                    CssClass="btn btn-danger btn-small" style="margin-top:0.5rem;"
                                    OnClientClick="return confirm('Unenroll from this course? Your lesson progress will be reset your quiz history is kept.');">Unenroll</asp:LinkButton>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
   <asp:Panel ID="litNoEnrollments" runat="server" Visible="false">
    <p>You haven't enrolled in any courses yet. <a href="<%= ResolveUrl("~/Pages/Courses.aspx") %>">Browse courses &rarr;</a></p>
</asp:Panel>

    <h2 style="margin-top:2rem;">Quiz History</h2>
    <div class="admin-panel">
        <asp:GridView ID="gvAttempts" runat="server" AutoGenerateColumns="false" CssClass="table-admin">
            <Columns>
                <asp:BoundField DataField="QuizTitle" HeaderText="Quiz" />
                <asp:TemplateField HeaderText="Score">
                    <ItemTemplate><%# Eval("Score") %> / <%# Eval("TotalMarks") %> (<%# Eval("PercentScore", "{0:0}") %>%)</ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Result">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("IsPassed") ? "badge" : "badge badge-draft" %>'>
                            <%# (bool)Eval("IsPassed") ? "Passed" : "Not passed" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AttemptedAt" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>
        <asp:Panel ID="litNoAttempts" runat="server" Visible="false"><p>No quiz attempts yet.</p></asp:Panel>
    </div>
</asp:Content>
