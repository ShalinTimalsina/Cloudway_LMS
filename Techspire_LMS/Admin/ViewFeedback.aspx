<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ViewFeedback.aspx.cs" Inherits="Techspire_LMS.Admin.ViewFeedback" %>


<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Feedback</h1>
    <asp:Literal ID="litMessage" runat="server" />

    <div class="admin-panel">
        <asp:GridView ID="gvFeedback" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                      DataKeyNames="FeedbackID" OnRowCommand="gvFeedback_RowCommand" OnRowDataBound="gvFeedback_RowDataBound"
                      AllowPaging="true" PageSize="10" OnPageIndexChanging="gvFeedback_PageIndexChanging">
            <PagerStyle CssClass="table-pager" />
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate><span id="statusDot" runat="server" /></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SubmittedAt" HeaderText="Received" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="Name" HeaderText="From" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" />
                <asp:BoundField DataField="Message" HeaderText="Message" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkMarkRead" runat="server" CommandName="MarkRead" CommandArgument='<%# Eval("FeedbackID") %>' CssClass="btn btn-outline btn-small">Mark read</asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("FeedbackID") %>' CssClass="btn btn-danger btn-small"
                                        OnClientClick="return confirm('Delete this feedback item?');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Literal ID="litEmpty" runat="server" Visible="false"><p>No feedback submitted yet.</p></asp:Literal>
    </div>
</asp:Content>
