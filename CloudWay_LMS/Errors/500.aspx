<%@ Page Title="Server Error" Language="C#" MasterPageFile="~/Masterpages/Site.Master" AutoEventWireup="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center" style="padding: 100px 20px;">
        <h1 style="font-size: 80px; color: #d32f2f;">500</h1>
        <h2>Internal Server Error.</h2>
        <p>Something went wrong on our end. We're looking into it.</p>
        <a href="~/Pages/Default.aspx" runat="server" class="btn btn-primary" style="margin-top: 20px;">Go to Homepage</a>
    </div>
</asp:Content>
