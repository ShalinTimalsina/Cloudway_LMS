<%@ Page Language="C#" MasterPageFile="~/Masterpages/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CloudWay_LMS.Account.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-card">
        <h1 class="page-title" style="text-align:center; margin-bottom: 1.5rem;">Log In</h1>
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

        <div class="form-field" style="display:flex; align-items:center; gap: 0.5rem; flex-direction:row;">
            <asp:CheckBox ID="chkRememberMe" runat="server" />
            <label for="<%= chkRememberMe.ClientID %>" style="margin:0; font-weight:400;">Remember me</label>
        </div>
        <asp:Button ID="btnLogin" runat="server" Text="Log In" CssClass="btn" OnClick="btnLogin_Click" />
        <p class="form-hint">No account? <a href="<%= ResolveUrl("~/Account/Register.aspx") %>">Register here</a>.</p>
    </div>
</asp:Content>



