<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.Master" CodeBehind="Profile.aspx.cs" Inherits="CloudWay_LMS.Account.Profile" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="page-title">My Profile</h1>

    <%-- Two independent forms share one page, so each gets its own
         ValidationGroup. Without this, clicking "Save changes" would ALSO
         run the (empty) password fields' RequiredFieldValidators below and
         block the postback with unrelated errors — a classic Web Forms trap
         when more than one button/section exists on a single page. --%>
    <div class="form-card">
        <h3>Profile details</h3>
        <asp:Literal ID="litProfileMessage" runat="server" />
        <asp:ValidationSummary ID="valProfileSummary" runat="server" ValidationGroup="ProfileForm"
                               CssClass="validation-summary" HeaderText="Please fix the following:" DisplayMode="BulletList" />

        <div class="form-field">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true" style="background:#f3f4f7;" />
            <span class="form-hint">Email can't be changed here - contact an administrator.</span>
        </div>
        <div class="form-field">
            <label for="<%= txtFullName.ClientID %>">Full name</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" MaxLength="100" ValidationGroup="ProfileForm" />
            <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                ValidationGroup="ProfileForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Full name is required." />
        </div>
        <div class="form-field">
            <label for="<%= txtPhone.ClientID %>">Phone</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" ValidationGroup="ProfileForm" />
        </div>

        <asp:Button ID="btnSaveProfile" runat="server" Text="Save changes" CssClass="btn"
                    ValidationGroup="ProfileForm" OnClick="btnSaveProfile_Click" />
    </div>

    <div class="form-card" style="margin-top:1.5rem;">
        <h3>Change password</h3>
        <asp:Literal ID="litPasswordMessage" runat="server" />
        <asp:ValidationSummary ID="valPasswordSummary" runat="server" ValidationGroup="PasswordForm"
                               CssClass="validation-summary" HeaderText="Please fix the following:" DisplayMode="BulletList" />

        <div class="form-field">
            <label for="<%= txtCurrentPassword.ClientID %>">Current password</label>
            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" ValidationGroup="PasswordForm" />
            <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server" ControlToValidate="txtCurrentPassword"
                ValidationGroup="PasswordForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Current password is required." />
        </div>
        <div class="form-field">
            <label for="<%= txtNewPassword.ClientID %>">New password</label>
            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" ValidationGroup="PasswordForm" />
            <span class="form-hint">At least 8 characters.</span>
            <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" ControlToValidate="txtNewPassword"
                ValidationGroup="PasswordForm" CssClass="field-error" Display="Dynamic" ErrorMessage="New password is required." />
            <asp:RegularExpressionValidator ID="revNewPassword" runat="server" ControlToValidate="txtNewPassword"
                ValidationGroup="PasswordForm" CssClass="field-error" Display="Dynamic"
                ErrorMessage="New password must be at least 8 characters." ValidationExpression=".{8,}" />
        </div>
        <div class="form-field">
            <label for="<%= txtConfirmNewPassword.ClientID %>">Confirm new password</label>
            <asp:TextBox ID="txtConfirmNewPassword" runat="server" CssClass="form-control" TextMode="Password" ValidationGroup="PasswordForm" />
            <asp:RequiredFieldValidator ID="rfvConfirmNewPassword" runat="server" ControlToValidate="txtConfirmNewPassword"
                ValidationGroup="PasswordForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Please confirm your new password." />
            <asp:CompareValidator ID="cvNewPasswordMatch" runat="server" ControlToValidate="txtConfirmNewPassword"
                ControlToCompare="txtNewPassword" ValidationGroup="PasswordForm" CssClass="field-error" Display="Dynamic"
                ErrorMessage="Passwords do not match." />
        </div>

        <asp:Button ID="btnChangePassword" runat="server" Text="Change password" CssClass="btn"
                    ValidationGroup="PasswordForm" OnClick="btnChangePassword_Click" />
    </div>
</asp:Content>
