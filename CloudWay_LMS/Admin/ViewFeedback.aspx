<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ViewFeedback.aspx.cs" Inherits="CloudWay_LMS.Admin.ViewFeedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 2rem;">
        <div>
            <h1 class="page-title" style="margin:0; font-family: var(--font-heading); font-size: 2rem; font-weight: 600; color: var(--text);">Student Feedback</h1>
            <p style="margin: 0.5rem 0 0; color: var(--text-muted); font-size: 15px;">Review and manage inquiries and course feedback from users.</p>
        </div>
    </div>
    <asp:Literal ID="litMessage" runat="server" />

    <div class="card" style="padding: 2rem; overflow-x: auto; min-height: 50vh;">
        <asp:GridView ID="gvFeedback" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                      DataKeyNames="FeedbackID" OnRowCommand="gvFeedback_RowCommand" OnRowDataBound="gvFeedback_RowDataBound"
                      AllowPaging="true" PageSize="10" OnPageIndexChanging="gvFeedback_PageIndexChanging" GridLines="None">
            <PagerStyle CssClass="table-pager" />
            <Columns>
                <asp:TemplateField HeaderText="Status" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <span id="statusDot" runat="server" style="display:inline-block; width: 10px; height: 10px; border-radius: 50%; box-shadow: 0 0 0 2px rgba(255,255,255,0.5);" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SubmittedAt" HeaderText="Received" DataFormatString="{0:MMM dd, yyyy}" ItemStyle-CssClass="text-muted" ItemStyle-Width="120px" />
                <asp:TemplateField HeaderText="Sender" ItemStyle-Width="200px">
                    <ItemTemplate>
                        <div style="font-weight: 500; color: var(--text);"><%# Eval("Name") %></div>
                        <div style="font-size: 13px; color: var(--text-muted);"><%# Eval("Email") %></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Subject" HeaderText="Subject" ItemStyle-Font-Bold="true" ItemStyle-Width="200px" />
                <asp:TemplateField HeaderText="Message">
                    <ItemTemplate>
                        <div style="max-width: 300px; color: var(--text-muted); font-size: 14px; line-height: 1.5;"><%# Eval("Message") %></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-Width="160px">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:flex-end; gap:0.5rem;">
                            <asp:LinkButton ID="lnkMarkRead" runat="server" CommandName="MarkRead" CommandArgument='<%# Eval("FeedbackID") %>' CssClass="btn btn-outline btn-small">Mark read</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("FeedbackID") %>' CssClass="btn btn-danger btn-small"
                                            OnClientClick="return confirm('Are you sure you want to permanently delete this feedback?');">Delete</asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        
        <asp:PlaceHolder ID="litEmpty" runat="server" Visible="false">
            <div style="text-align: center; padding: 4rem 2rem;">
                <div style="background: rgba(11, 37, 69, 0.05); width: 64px; height: 64px; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 1rem; color: var(--brand);">
                    <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path></svg>
                </div>
                <h3 style="margin: 0 0 0.5rem; color: var(--text);">No Feedback Yet</h3>
                <p style="margin: 0; color: var(--text-muted);">You're all caught up! No feedback has been submitted by students.</p>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
