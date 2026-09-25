<%@ Page Language="C#" AutoEventWireup="true"MasterPageFile="~/Masterpages/Site.master" CodeBehind="Quiz.aspx.cs" Inherits="CloudWay_LMS.Member.Quiz" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
        <div class="alert alert-error">This quiz is not available.</div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phQuizForm" runat="server">
        <h1 class="page-title"><asp:Literal ID="litQuizTitle" runat="server" /></h1>
        <p class="page-subtitle">Pass mark: <asp:Literal ID="litPassMark" runat="server" />% 
           <asp:Literal ID="litTimeLimit" runat="server" />
        </p>
        <div style="margin-bottom:20px; color:var(--muted);"><asp:Literal ID="litQuizDesc" runat="server" /></div>
        <asp:Literal ID="litMessage" runat="server" />

        <%-- Each question renders its own CheckBoxList of options. Grading in
             QuizBLL.SubmitAttempt is exact-set-match, so this same markup works
             unmodified for SingleChoice, TrueFalse and MultipleChoice questions
             — swap to a RadioButtonList here only if you want the BROWSER to
             also stop a learner multi-selecting a single-answer question;
             the server-side scoring rule does not depend on which control you use. --%>
        <asp:Repeater ID="rptQuestions" runat="server" OnItemDataBound="rptQuestions_ItemDataBound">
            <ItemTemplate>
                <div class="quiz-question">
                    <asp:HiddenField ID="hfQuestionId" runat="server" Value='<%# Eval("QuestionID") %>' />
                    <asp:HiddenField ID="hfQuestionType" runat="server" Value='<%# Eval("QuestionType") %>' />
                    <h4><%# Container.ItemIndex + 1 %>. <%# Eval("QuestionText") %></h4>
                    
                    <asp:PlaceHolder ID="phOptions" runat="server">
                        <asp:CheckBoxList ID="cblOptions" runat="server" />
                    </asp:PlaceHolder>
                    
                    <asp:PlaceHolder ID="phSingleAnswer" runat="server" Visible="false">
                        <asp:TextBox ID="txtSingleAnswer" runat="server" CssClass="form-control" placeholder="Type your answer here..." />
                    </asp:PlaceHolder>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit Quiz" CssClass="btn" OnClick="btnSubmit_Click" />
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phResult" runat="server" Visible="false">
        <div class="quiz-result">
            <p class="quiz-score"><asp:Literal ID="litScore" runat="server" /></p>
            <p><asp:Literal ID="litOutcome" runat="server" /></p>
            <a class="btn btn-outline" href="<%= ResolveUrl("~/Pages/Courses.aspx") %>">Back to courses</a>
        </div>
    </asp:PlaceHolder>
</asp:Content>

