<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masterpages/Site.master" CodeBehind="Contact.aspx.cs" Inherits="Techspire_LMS.Pages.Contact" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="page-title">Contact Us</h1>
    <p class="page-subtitle">Questions, suggestions, or something not working? Let us know.</p>

    <asp:PlaceHolder ID="phForm" runat="server">
        <div class="form-card">
            <asp:Literal ID="litMessage" runat="server" />
            <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                                   HeaderText="Please fix the following:" DisplayMode="BulletList" />

            <div class="form-field">
                <label for="<%= txtName.ClientID %>">Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="100" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                    CssClass="field-error" Display="Dynamic" ErrorMessage="Name is required." />
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
                <label for="<%= txtSubject.ClientID %>">Subject</label>
                <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" MaxLength="150" />
                <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                    CssClass="field-error" Display="Dynamic" ErrorMessage="Subject is required." />
            </div>
            <div class="form-field">
                <label for="<%= txtMessage.ClientID %>">Message</label>
                <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                <asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage"
                    CssClass="field-error" Display="Dynamic" ErrorMessage="Message is required." />
            </div>

            <asp:Button ID="btnSubmit" runat="server" Text="Send" CssClass="btn" OnClick="btnSubmit_Click" />
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phThanks" runat="server" Visible="false">
        <div class="alert alert-success">Thanks — we've received your message and will get back to you.</div>
    </asp:PlaceHolder>
</asp:Content>
