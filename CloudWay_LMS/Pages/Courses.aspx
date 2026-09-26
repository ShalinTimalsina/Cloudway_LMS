<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="Courses.aspx.cs" Inherits="CloudWay_LMS.Pages.Courses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Breadcrumbs -->
    <div class="breadcrumb-bar">
        <a href="<%= ResolveUrl("~/Pages/Default.aspx") %>">Home</a>
        <span class="breadcrumb-separator"><svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg></span>
        <span class="breadcrumb-current">All Courses</span>
    </div>

    <h1 class="page-title">Courses</h1>
    <p class="page-subtitle">Filter by category or search by keyword.</p>

    <div class="card" style="padding:1.5rem; margin-bottom:2rem; display:flex; flex-direction:row; align-items:flex-end; gap:1.5rem; flex-wrap:wrap;">
        <div class="form-field" style="margin:0; flex:1; min-width:200px;">
            <label for="<%= ddlCategory.ClientID %>">Category</label>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"
                              AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
        </div>
        <div class="form-field" style="margin:0; flex:2; min-width:250px;">
            <label for="<%= txtSearch.ClientID %>">Search</label>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Keyword..." />
        </div>
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn" OnClick="Filter_Changed" style="margin:0;" />
    </div>

    <asp:PlaceHolder ID="phTagFilter" runat="server" Visible="false">
        <p class="card-meta" style="margin-bottom:1rem;">
            Filtered by tag: <span class="tag"><asp:Literal ID="litActiveTag" runat="server" /></span>
            &nbsp;<asp:LinkButton ID="lnkClearTag" runat="server" OnClick="lnkClearTag_Click" CausesValidation="false">&times; clear</asp:LinkButton>
        </p>
    </asp:PlaceHolder>

    <asp:Repeater ID="rptCourses" runat="server">
        <HeaderTemplate><div class="card-grid"></HeaderTemplate>
        <ItemTemplate>
            <article class="card card-clickable" onclick="window.location.href='<%# ResolveUrl("~/Pages/CourseDetails.aspx?id=") + Eval("CourseID") %>';">
                <img class="card-thumb" src="<%# ResolveUrl((string)Eval("ThumbnailOrDefault")) %>" alt="" />
                <div class="card-body">
                    <span class="badge"><%# Eval("CategoryName") %></span>
                    <h3 class="card-title"><%# Eval("Title") %></h3>
                    <p class="card-meta">Self-paced</p>
                    <p class="card-desc"><%# Eval("Description") %></p>
                    <a class="btn btn-outline btn-small"
                       href="<%# ResolveUrl("~/Pages/CourseDetails.aspx?id=") + Eval("CourseID") %>">View course</a>
                </div>
            </article>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>

    <asp:Literal ID="litEmpty" runat="server" Visible="false">
        <p>No courses match your filter.</p>
    </asp:Literal>

    <asp:PlaceHolder ID="phPager" runat="server" Visible="false">
        <div style="display:flex; align-items:center; gap:1rem; justify-content:center; margin-top:1.5rem;">
            <asp:Button ID="btnPrev" runat="server" Text="&larr; Previous" CssClass="btn btn-outline btn-small"
                        OnClick="btnPrev_Click" CausesValidation="false" />
            <span class="card-meta"><asp:Literal ID="litPageStatus" runat="server" /></span>
            <asp:Button ID="btnNext" runat="server" Text="Next &rarr;" CssClass="btn btn-outline btn-small"
                        OnClick="btnNext_Click" CausesValidation="false" />
        </div>
    </asp:PlaceHolder>
</asp:Content>
