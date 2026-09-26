<%@ Page Language="C#" AutoEventWireup="true"MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageUsers.aspx.cs" Inherits="CloudWay_LMS.Admin.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 2rem;">
        <div>
            <h1 class="page-title" style="margin:0; font-family: var(--font-heading); font-size: 2rem; font-weight: 600; color: var(--text);">User Management</h1>
            <p style="margin: 0.5rem 0 0; color: var(--text-muted); font-size: 15px;">Monitor and manage student and instructor accounts.</p>
        </div>
    </div>
    
    <asp:Literal ID="litMessage" runat="server" />

    <div class="card" style="padding: 2rem; margin-bottom: 2rem; overflow-x: auto; min-height: 50vh;">
        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 1.5rem;">
            <h3 style="margin: 0; font-family: var(--font-heading); font-size: 1.25rem; font-weight: 600;">Registered Users</h3>
        </div>
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                      DataKeyNames="UserID" OnRowCommand="gvUsers_RowCommand" OnRowDataBound="gvUsers_RowDataBound"
                      AllowPaging="true" PageSize="10" OnPageIndexChanging="gvUsers_PageIndexChanging" GridLines="None">
            <PagerStyle CssClass="table-pager" />
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <div style="font-weight: 500; color: var(--text);"><%# Eval("FullName") %></div>
                        <div style="font-size: 13px; color: var(--text-muted);"><%# Eval("Email") %></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Role">
                    <ItemTemplate>
                        <span class="badge" style="background: rgba(11, 37, 69, 0.08); color: var(--brand); font-size: 12px; padding: 4px 10px; border-radius: 99px;">
                            <%# Eval("RoleName") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='<%# (bool)Eval("IsActive") ? "badge badge-success" : "badge badge-danger" %>' style="font-size: 12px; padding: 4px 10px; border-radius: 99px;">
                            <%# (bool)Eval("IsActive") ? "Active" : "Deactivated" %>
                        </span>
                        <asp:Literal ID="litLocked" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:flex-end; gap:0.5rem;">
                            <asp:LinkButton ID="lnkRole" runat="server" CommandName="ToggleRole" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-outline btn-small" />
                            <asp:LinkButton ID="lnkStatus" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-outline btn-small" />
                            <asp:LinkButton ID="lnkUnlock" runat="server" CommandName="ClearLockout" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-outline btn-small" Text="Unlock" Visible="false" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
