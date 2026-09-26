<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageCategories.aspx.cs" Inherits="CloudWay_LMS.Admin.ManageCategories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 2rem;">
        <h1 class="page-title" style="margin:0;">Manage Categories</h1>
    </div>

    <asp:Literal ID="litMessage" runat="server" />
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="validation-summary"
                           HeaderText="Please fix the following:" DisplayMode="BulletList" />

    <style>
        .text-muted { color: var(--text-muted); }
        .categories-layout {
            display: grid;
            grid-template-columns: 1fr;
            gap: 2rem;
            align-items: start;
        }
        @media (min-width: 992px) {
            .categories-layout {
                grid-template-columns: 350px 1fr;
            }
            .form-sticky {
                position: sticky;
                top: 2rem;
            }
        }
    </style>

    <div class="categories-layout">
        <!-- Sidebar Form -->
        <div class="card form-sticky" style="padding: 2rem;">
            <asp:HiddenField ID="hfCategoryId" runat="server" Value="0" />
            <h3 style="margin-top: 0; margin-bottom: 1.5rem; font-family: var(--font-heading); font-size: 1.25rem; font-weight: 600;">
                <asp:Literal ID="litFormTitle" runat="server" Text="Add New Category" />
            </h3>

            <div class="form-field">
                <label for="<%= txtName.ClientID %>" style="font-size: 14px; color: var(--text-muted); margin-bottom: 0.5rem; display: block;">Category Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="60" placeholder="e.g. Web Development" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                    CssClass="field-error" Display="Dynamic" ErrorMessage="Category name is required." />
            </div>
            <div class="form-field">
                <label for="<%= txtDescription.ClientID %>" style="font-size: 14px; color: var(--text-muted); margin-bottom: 0.5rem; display: block;">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="250" placeholder="Briefly describe the category..." style="height: auto; padding-top: 10px;" />
            </div>
            
            <div style="display: flex; gap: 0.75rem; margin-top: 2rem;">
                <asp:Button ID="btnSave" runat="server" Text="Save Category" CssClass="btn" OnClick="btnSave_Click" style="flex: 1;" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" style="flex: 1;" />
            </div>
        </div>

        <!-- Main Content -->
        <div class="card" style="padding: 2rem; overflow-x: auto;">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 1.5rem;">
                <h3 style="margin: 0; font-family: var(--font-heading); font-size: 1.25rem; font-weight: 600;">Active Categories</h3>
            </div>
            <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" CssClass="table-admin"
                          DataKeyNames="CategoryID" OnRowCommand="gvCategories_RowCommand"
                          AllowPaging="True" OnPageIndexChanging="gvCategories_PageIndexChanging" PageSize="10" GridLines="None">
                <PagerStyle CssClass="table-pager" />
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Category Name" ItemStyle-Font-Bold="true" />
                    <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-CssClass="text-muted" />
                    <asp:BoundField DataField="CourseCount" HeaderText="Courses" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    
                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <div style="display:flex; justify-content:flex-end; gap:0.5rem;">
                                <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("CategoryID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("CategoryID") %>' CssClass="btn btn-danger btn-small" CausesValidation="false"
                                                OnClientClick="return confirm('Are you sure you want to delete this category?');">Delete</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
