<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageCategories.aspx.cs" Inherits="Techspire_LMS.Admin.ManageCategories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Categories</h1>
    <asp:Literal ID="litMessage" runat="server" />
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                           HeaderText="Please fix the following:" DisplayMode="BulletList" />

    <div class="admin-panel">
        <asp:HiddenField ID="hfCategoryId" runat="server" Value="0" />
        <h3><asp:Literal ID="litFormTitle" runat="server" Text="Add a category" /></h3>

        <div class="form-field">
            <label for="<%= txtName.ClientID %>">Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="60" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                CssClass="field-error" Display="Dynamic" ErrorMessage="Category name is required." />
        </div>
        <div class="form-field">
            <label for="<%= txtDescription.ClientID %>">Description</label>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="250" />
        </div>
        <div class="form-field">
            <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active (visible to learners)" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
    </div>

    <div class="admin-panel">
        <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" CssClass="table-admin"
                      DataKeyNames="CategoryID" OnRowCommand="gvCategories_RowCommand"
                      AllowPaging="True" OnPageIndexChanging="gvCategories_PageIndexChanging">
            <PagerStyle CssClass="table-pager" />
            <Columns>
                <asp:BoundField DataField="CategoryName" HeaderText="Name" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="CourseCount" HeaderText="Courses" />
                <asp:TemplateField HeaderText="Active">
                    <ItemTemplate><%# (bool)Eval("IsActive") ? "Yes" : "No" %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("CategoryID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("CategoryID") %>' CssClass="btn btn-danger btn-small" CausesValidation="false"
                                        OnClientClick="return confirm('Delete this category?');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
