<%@ Page Language="C#" MasterPageFile="~/Masterpages/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Techspire_LMS.Account.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="page-title">Log In</h1>

    <div class="form-card">
        <asp:Literal ID="litMessage" runat="server" />
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                               HeaderText="Please fix the following:" DisplayMode="BulletList" />

        <div class="form-field">
            <label for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Email is required." />
        </div>
        <div class="form-field">
            <label for="<%= txtPassword.ClientID %>">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Password is required." />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Log In" CssClass="btn" OnClick="btnLogin_Click" />
        <p class="form-hint">No account? <a href="<%= ResolveUrl("~/Account/Register.aspx") %>">Register here</a>.</p>
    </div>
</asp:Content>

