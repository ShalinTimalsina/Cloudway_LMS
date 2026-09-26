<%@ Page Language="C#" MasterPageFile="~/Masterpages/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CloudWay_LMS.Account.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-card">
        <h1 class="page-title" style="text-align:center; margin-bottom: 1.5rem;">Log In</h1>
        <asp:Literal ID="litMessage" runat="server" />
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                               HeaderText="Please fix the following:" DisplayMode="BulletList" />

        <div class="form-field">
            <label for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" Text="admin@cloudway.local" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Email is required." />
        </div>
        <div class="form-field">
            <label for="<%= txtPassword.ClientID %>">Password</label>
            <div class="pwd-container">
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" style="width:100%;" />
                <button type="button" class="pwd-toggle" onclick="togglePassword(this)" aria-label="Toggle password visibility">
                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                </button>
            </div>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Password is required." />
        </div>

        <div class="form-field" style="display:flex; align-items:center; gap: 0.5rem; flex-direction:row;">
            <asp:CheckBox ID="chkRememberMe" runat="server" />
            <label for="<%= chkRememberMe.ClientID %>" style="margin:0; font-weight:400;">Remember me</label>
        </div>
        <asp:Button ID="btnLogin" runat="server" Text="Log In" CssClass="btn" style="width: 100%; margin-top: 1rem;" OnClick="btnLogin_Click" />
        <div style="margin-top:1.5rem; padding-top:1rem; border-top:1px solid var(--border); text-align:center;">
            <p class="form-hint" style="margin-bottom:0.5rem;">Quick Test Login:</p>
            <div style="display:flex; gap:0.5rem; justify-content:center;">
                <button type="button" class="btn btn-outline btn-small" onclick="document.getElementById('<%= txtEmail.ClientID %>').value='admin@cloudway.local'; document.getElementById('<%= txtPassword.ClientID %>').value='Admin123!'; return false;">Fill Admin</button>
                <button type="button" class="btn btn-outline btn-small" onclick="document.getElementById('<%= txtEmail.ClientID %>').value='student@cloudway.local'; document.getElementById('<%= txtPassword.ClientID %>').value='Student123!'; return false;">Fill Student</button>
            </div>
        </div>
        <p class="form-hint" style="margin-top:1.5rem; text-align:center;">No account? <a href="<%= ResolveUrl("~/Account/Register.aspx") %>">Register here</a>.</p>
    </div>
        <script>
            document.addEventListener("DOMContentLoaded", function () {
                var txtPwd = document.getElementById('<%= txtPassword.ClientID %>');
                if (txtPwd && !txtPwd.value) {
                    txtPwd.value = 'Admin123!';
                }
            });
        </script>
</asp:Content>




