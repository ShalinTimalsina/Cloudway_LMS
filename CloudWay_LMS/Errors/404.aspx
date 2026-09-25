<%@ Page Title="Page Not Found" Language="C#" MasterPageFile="~/Masterpages/Site.Master" AutoEventWireup="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center" style="padding: 100px 20px;">
        <h1 style="font-size: 80px; color: #102a43;">404</h1>
        <h2>Oops! Page not found.</h2>
        <p>The page you are looking for might have been removed, had its name changed, or is temporarily unavailable.</p>
        <a href="~/Pages/Default.aspx" runat="server" class="btn btn-primary" style="margin-top: 20px;">Go to Homepage</a>
    </div>
</asp:Content>
