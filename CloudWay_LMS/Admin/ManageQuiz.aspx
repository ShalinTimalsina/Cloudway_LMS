<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master"  CodeBehind="ManageQuiz.aspx.cs" Inherits="CloudWay_LMS.Admin.ManagaeQuiz" %>


<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Quizzes</h1>
    <p class="page-subtitle">Play and win, Folks</p>
    <asp:Literal ID="litMessage" runat="server" />

    <asp:HiddenField ID="hfQuizId" runat="server" Value="0" />
    <asp:HiddenField ID="hfQuestionId" runat="server" Value="0" />

    <%-- ============== LEVEL 1: course -> quizzes ============== --%>
    <div class="admin-panel">
        <div class="form-field" style="max-width:400px;">
            <label for="<%= ddlCourse.ClientID %>">Course</label>
            <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" CausesValidation="false"
                              AutoPostBack="true" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged" />
        </div>
    </div>

    <asp:PlaceHolder ID="phQuizPanel" runat="server" Visible="false">
        <div class="admin-panel">
            <h3><asp:Literal ID="litQuizFormTitle" runat="server" Text="Add a quiz" /></h3>
            <asp:ValidationSummary ID="valQuizSummary" runat="server" ValidationGroup="QuizForm"
                                   CssClass="validation-summary" HeaderText="Please fix the following:" DisplayMode="BulletList" />
            <div class="form-field" style="max-width:400px;">
                <label for="<%= txtQuizTitle.ClientID %>">Quiz title</label>
                <asp:TextBox ID="txtQuizTitle" runat="server" CssClass="form-control" MaxLength="150" ValidationGroup="QuizForm" />
                <asp:RequiredFieldValidator ID="rfvQuizTitle" runat="server" ControlToValidate="txtQuizTitle"
                    ValidationGroup="QuizForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Quiz title is required." />
            </div>
            <div class="form-field" style="max-width:200px;">
                <label for="<%= txtPassingScore.ClientID %>">Pass mark (%)</label>
                <asp:TextBox ID="txtPassingScore" runat="server" CssClass="form-control" TextMode="Number" Text="50" />
            </div>
            <div class="form-field">
                <asp:CheckBox ID="chkQuizActive" runat="server" Checked="true" Text="Active" />
            </div>
            <asp:Button ID="btnSaveQuiz" runat="server" Text="Save Quiz" CssClass="btn" ValidationGroup="QuizForm" OnClick="btnSaveQuiz_Click" />
            <asp:Button ID="btnCancelQuiz" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancelQuiz_Click" CausesValidation="false" />
        </div>

        <div class="admin-panel">
            <asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                          DataKeyNames="QuizID" OnRowCommand="gvQuizzes_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:BoundField DataField="PassingScore" HeaderText="Pass %" />
                    <asp:BoundField DataField="QuestionCount" HeaderText="Questions" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="ManageQuestions" CommandArgument='<%# Eval("QuizID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Manage Questions</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("QuizID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("QuizID") %>' CssClass="btn btn-danger btn-small"
                                            CausesValidation="false" OnClientClick="return confirm('Delete this quiz and all its questions?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:PlaceHolder>

    <%-- ============== LEVEL 2: quiz -> questions ============== --%>
    <asp:PlaceHolder ID="phQuestionPanel" runat="server" Visible="false">
        <div class="admin-panel">
            <h3>Questions for "<asp:Literal ID="litCurrentQuizTitle" runat="server" />"</h3>
            <asp:ValidationSummary ID="valQuestionSummary" runat="server" ValidationGroup="QuestionForm"
                                   CssClass="validation-summary" HeaderText="Please fix the following:" DisplayMode="BulletList" />
            <div class="form-field">
                <label for="<%= txtQuestionText.ClientID %>">Question text</label>
                <asp:TextBox ID="txtQuestionText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" ValidationGroup="QuestionForm" />
                <asp:RequiredFieldValidator ID="rfvQuestionText" runat="server" ControlToValidate="txtQuestionText"
                    ValidationGroup="QuestionForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Question text is required." />
            </div>
            <div class="form-field" style="max-width:220px;">
                <label for="<%= ddlQuestionType.ClientID %>">Type</label>
                <asp:DropDownList ID="ddlQuestionType" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Single choice" Value="SingleChoice" />
                    <asp:ListItem Text="Multiple choice" Value="MultipleChoice" />
                    <asp:ListItem Text="True / False" Value="TrueFalse" />
                </asp:DropDownList>
            </div>
            <div class="form-field" style="max-width:150px;">
                <label for="<%= txtMarks.ClientID %>">Marks</label>
                <asp:TextBox ID="txtMarks" runat="server" CssClass="form-control" TextMode="Number" Text="1" />
            </div>
            <asp:Button ID="btnSaveQuestion" runat="server" Text="Save Question" CssClass="btn" ValidationGroup="QuestionForm" OnClick="btnSaveQuestion_Click" />
            <asp:Button ID="btnCancelQuestion" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancelQuestion_Click" CausesValidation="false" />
            <asp:Button ID="btnCloseQuestionPanel" runat="server" Text="&larr; Back to quizzes" CssClass="btn btn-outline" OnClick="btnCloseQuestionPanel_Click" CausesValidation="false" />
        </div>

        <div class="admin-panel">
            <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                          DataKeyNames="QuestionID" OnRowCommand="gvQuestions_RowCommand">
                <Columns>
                    <asp:BoundField DataField="QuestionText" HeaderText="Question" />
                    <asp:BoundField DataField="QuestionType" HeaderText="Type" />
                    <asp:BoundField DataField="Marks" HeaderText="Marks" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="ManageOptions" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Manage Options</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-danger btn-small"
                                            CausesValidation="false" OnClientClick="return confirm('Delete this question and its options?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:PlaceHolder>

    <%-- ============== LEVEL 3: question -> options ============== --%>
    <asp:PlaceHolder ID="phOptionPanel" runat="server" Visible="false">
        <div class="admin-panel">
            <asp:HiddenField ID="hfOptionEditId" runat="server" Value="0" />
            <h3>Options for: "<asp:Literal ID="litCurrentQuestionText" runat="server" />"</h3>
            <asp:ValidationSummary ID="valOptionSummary" runat="server" ValidationGroup="OptionForm"
                                   CssClass="validation-summary" HeaderText="Please fix the following:" DisplayMode="BulletList" />
            <div class="form-field">
                <label for="<%= txtOptionText.ClientID %>">Option text</label>
                <asp:TextBox ID="txtOptionText" runat="server" CssClass="form-control" MaxLength="250" ValidationGroup="OptionForm" />
                <asp:RequiredFieldValidator ID="rfvOptionText" runat="server" ControlToValidate="txtOptionText"
                    ValidationGroup="OptionForm" CssClass="field-error" Display="Dynamic" ErrorMessage="Option text is required." />
            </div>
            <div class="form-field">
                <asp:CheckBox ID="chkIsCorrect" runat="server" Text="This is a correct answer" />
            </div>
            <div class="form-field" style="max-width:150px;">
                <label for="<%= txtOptionSort.ClientID %>">Sort order</label>
                <asp:TextBox ID="txtOptionSort" runat="server" CssClass="form-control" TextMode="Number" Text="1" />
            </div>
            <asp:Button ID="btnSaveOption" runat="server" Text="Save Option" CssClass="btn" ValidationGroup="OptionForm" OnClick="btnSaveOption_Click" />
            <asp:Button ID="btnCancelOption" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancelOption_Click" CausesValidation="false" />
            <asp:Button ID="btnCloseOptionPanel" runat="server" Text="&larr; Back to questions" CssClass="btn btn-outline" OnClick="btnCloseOptionPanel_Click" CausesValidation="false" />
        </div>

        <div class="admin-panel">
            <asp:GridView ID="gvOptions" runat="server" AutoGenerateColumns="false" CssClass="table-admin"
                          DataKeyNames="OptionID" OnRowCommand="gvOptions_RowCommand">
                <Columns>
                    <asp:BoundField DataField="SortOrder" HeaderText="#" />
                    <asp:BoundField DataField="OptionText" HeaderText="Option" />
                    <asp:TemplateField HeaderText="Correct">
                        <ItemTemplate><%# (bool)Eval("IsCorrect") ? "✔" : "" %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("OptionID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("OptionID") %>' CssClass="btn btn-danger btn-small"
                                            CausesValidation="false" OnClientClick="return confirm('Delete this option?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:PlaceHolder>
</asp:Content>
