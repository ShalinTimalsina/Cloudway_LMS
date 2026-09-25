<%@ Page Language="C#" AutoEventWireup="true"MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageUsers.aspx.cs" Inherits="CloudWay_LMS.Admin.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Users</h1>
    <asp:Literal ID="litMessage" runat="server" />

    <div class="admin-panel">
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                      DataKeyNames="UserID" OnRowCommand="gvUsers_RowCommand" OnRowDataBound="gvUsers_RowDataBound"
                      AllowPaging="true" PageSize="10" OnPageIndexChanging="gvUsers_PageIndexChanging">
            <PagerStyle CssClass="table-pager" />
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="RoleName" HeaderText="Role" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("IsActive") ? "badge" : "badge badge-draft" %>'>
                            <%# (bool)Eval("IsActive") ? "Active" : "Deactivated" %>
                        </span>
                        <asp:Literal ID="litLocked" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkRole" runat="server" CommandName="ToggleRole" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-outline btn-small" />
                        <asp:LinkButton ID="lnkStatus" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-outline btn-small" />
                        <asp:LinkButton ID="lnkUnlock" runat="server" CommandName="ClearLockout" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-outline btn-small" Text="Unlock" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
