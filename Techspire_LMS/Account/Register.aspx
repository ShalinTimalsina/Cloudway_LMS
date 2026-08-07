<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="Register.aspx.cs" Inherits="Techspire_LMS.Account.Register" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="page-title">Create an Account</h1>

    <div class="form-card">
        <asp:Literal ID="litMessage" runat="server" />

        <%-- ValidationSummary + per-field validators are the CLIENT-side layer
             (F12) — they run in the browser via the WebForms validation
             script before a postback even happens, giving instant feedback.
             They are a UX convenience, NOT the security boundary: AuthBLL's
             server-side checks (Register.aspx.cs -> AuthBLL.Register) are
             what actually protects the data, and run regardless of whether
             JavaScript is enabled. Both layers exist on purpose — see
             README-Validation-Multimedia.md. --%>
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                               HeaderText="Please fix the following:" DisplayMode="BulletList" />

        <div class="form-field">
            <label for="<%= txtFullName.ClientID %>">Full name</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Full name is required." />
        </div>
        <div class="form-field">
            <label for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Email is required." />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Enter a valid email address."
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
        </div>
        <div class="form-field">
            <label for="<%= txtPhone.ClientID %>">Phone (optional)</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
        </div>
        <div class="form-field">
            <label for="<%= txtPassword.ClientID %>">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
            <span class="form-hint">At least 8 characters.</span>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Password is required." />
            <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Password must be at least 8 characters."
                ValidationExpression=".{8,}" />
        </div>
        <div class="form-field">
            <label for="<%= txtConfirmPassword.ClientID %>">Confirm password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Please confirm your password." />
            <asp:CompareValidator ID="cvPasswordMatch" runat="server" ControlToValidate="txtConfirmPassword"
                ControlToCompare="txtPassword" CssClass="field-error" Display="Dynamic"
                ErrorMessage="Passwords do not match." />
        </div>

        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn" OnClick="btnRegister_Click" />
        <p class="form-hint">Already have an account? <a href="<%= ResolveUrl("~/Account/Login.aspx") %>">Log in</a>.</p>
    </div>
</asp:Content>
