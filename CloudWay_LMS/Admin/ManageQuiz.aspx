<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" MasterPageFile="~/MasterPages/Admin.master" CodeBehind="ManageQuiz.aspx.cs" Inherits="CloudWay_LMS.Admin.ManagaeQuiz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <h1 class="page-title">Manage Quizzes</h1>
    
    <asp:Literal ID="litMessage" runat="server" />

    <asp:HiddenField ID="hfQuizData" runat="server" />

    <div id="quizListPanel" runat="server" class="admin-panel">
        <div class="form-field" style="max-width:400px;">
            <label>Select Course</label>
            <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged" />
        </div>
        
        <div runat="server" id="divQuizList" visible="false">
            <asp:Button ID="btnCreateQuiz" runat="server" Text="Create New Quiz" CssClass="btn" OnClick="btnCreateQuiz_Click" CausesValidation="false" style="margin-bottom:20px;" />
            <asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" CssClass="table-admin" DataKeyNames="QuizID" OnRowCommand="gvQuizzes_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:BoundField DataField="PassingScore" HeaderText="Pass %" />
                    <asp:TemplateField HeaderText="Published">
                        <ItemTemplate><%# (bool)Eval("IsPublished") ? "Yes" : "No" %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="EditQuiz" CommandArgument='<%# Eval("QuizID") %>' CssClass="btn btn-outline btn-small" CausesValidation="false">Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteQuiz" CommandArgument='<%# Eval("QuizID") %>' CssClass="btn btn-danger btn-small" CausesValidation="false" OnClientClick="return confirm('Delete this quiz and all questions?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div id="quizEditorPanel" runat="server" visible="false" style="max-width:800px;">
        <div class="admin-panel">
            <h3>Quiz Settings</h3>
            <div class="form-field">
                <label>Quiz Title</label>
                <input type="text" id="txtQuizTitle" class="form-control" maxlength="150" />
            </div>
            <div class="form-field">
                <label>Description / Instructions</label>
                <textarea id="txtQuizDesc" class="form-control" rows="3"></textarea>
            </div>
            <div class="form-field" style="max-width:200px;">
                <label>Time Limit (minutes)</label>
                <input type="number" id="txtTimeLimit" class="form-control" placeholder="Optional" />
            </div>
            <div class="form-field" style="max-width:200px;">
                <label>Passing Score (%)</label>
                <input type="number" id="txtPassingScore" class="form-control" value="50" />
            </div>
            <div class="form-field">
                <label><input type="checkbox" id="chkIsPublished" /> Publish this quiz</label>
            </div>
        </div>

        <h3>Questions</h3>
        <div id="questionsContainer"></div>
        <button type="button" class="btn btn-outline" style="margin-top:10px;" onclick="addQuestion()">+ Add Question</button>
        
        <div class="admin-panel" style="margin-top:40px;">
            <asp:Button ID="btnSaveQuiz" runat="server" Text="Save Quiz" CssClass="btn" OnClientClick="return saveQuizData();" OnClick="btnSaveQuiz_Click" />
            <asp:Button ID="btnCancelQuiz" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancelQuiz_Click" CausesValidation="false" />
        </div>
    </div>
    
    <script>
        var quizData = {
            QuizID: 0,
            CourseID: 0,
            Title: '',
            Description: '',
            TimeLimitMinutes: null,
            PassingScore: 50,
            IsPublished: false,
            Questions: []
        };
        
        function initEditor(jsonStr) {
            if (jsonStr) {
                quizData = JSON.parse(jsonStr);
                document.getElementById('txtQuizTitle').value = quizData.Title || '';
                document.getElementById('txtQuizDesc').value = quizData.Description || '';
                document.getElementById('txtTimeLimit').value = quizData.TimeLimitMinutes || '';
                document.getElementById('txtPassingScore').value = quizData.PassingScore || 50;
                document.getElementById('chkIsPublished').checked = quizData.IsPublished || false;
            } else {
                // Keep the existing CourseID if it's a new quiz
                var cid = quizData.CourseID;
                quizData = { QuizID: 0, CourseID: cid, Title: '', Description: '', TimeLimitMinutes: null, PassingScore: 50, IsPublished: false, Questions: [] };
                document.getElementById('txtQuizTitle').value = '';
                document.getElementById('txtQuizDesc').value = '';
                document.getElementById('txtTimeLimit').value = '';
                document.getElementById('txtPassingScore').value = '50';
                document.getElementById('chkIsPublished').checked = false;
            }
            renderQuestions();
        }
        
        function addQuestion() {
            quizData.Questions.push({
                QuestionID: 0,
                QuestionText: '',
                QuestionType: 'SingleChoice',
                Marks: 1,
                Options: [
                    { OptionID: 0, OptionText: '', IsCorrect: true },
                    { OptionID: 0, OptionText: '', IsCorrect: false }
                ]
            });
            renderQuestions();
        }
        
        function deleteQuestion(qIndex) {
            quizData.Questions.splice(qIndex, 1);
            renderQuestions();
        }
        
        function changeQuestionType(qIndex, type) {
            var q = quizData.Questions[qIndex];
            q.QuestionType = type;
            if (type === 'TrueFalse') {
                q.Options = [
                    { OptionID: q.Options[0] ? q.Options[0].OptionID : 0, OptionText: 'True', IsCorrect: true },
                    { OptionID: q.Options[1] ? q.Options[1].OptionID : 0, OptionText: 'False', IsCorrect: false }
                ];
            } else if (type === 'SingleAnswer') {
                q.Options = [
                    { OptionID: q.Options[0] ? q.Options[0].OptionID : 0, OptionText: q.Options[0] ? q.Options[0].OptionText : '', IsCorrect: true }
                ];
            } else if (type === 'SingleChoice') {
                if (q.Options.length < 2) {
                    q.Options.push({ OptionID: 0, OptionText: '', IsCorrect: false });
                }
            }
            renderQuestions();
        }
        
        function addOption(qIndex) {
            quizData.Questions[qIndex].Options.push({ OptionID: 0, OptionText: '', IsCorrect: false });
            renderQuestions();
        }
        
        function deleteOption(qIndex, oIndex) {
            quizData.Questions[qIndex].Options.splice(oIndex, 1);
            if (quizData.Questions[qIndex].Options.length === 0) {
                quizData.Questions[qIndex].Options.push({ OptionID: 0, OptionText: '', IsCorrect: true });
            } else {
                var hasCorrect = false;
                for (var i=0; i<quizData.Questions[qIndex].Options.length; i++) {
                    if (quizData.Questions[qIndex].Options[i].IsCorrect) hasCorrect = true;
                }
                if (!hasCorrect) quizData.Questions[qIndex].Options[0].IsCorrect = true;
            }
            renderQuestions();
        }
        
        function setCorrectOption(qIndex, oIndex) {
            var q = quizData.Questions[qIndex];
            if (q.QuestionType === 'SingleChoice' || q.QuestionType === 'TrueFalse') {
                for (var i=0; i<q.Options.length; i++) {
                    q.Options[i].IsCorrect = (i === oIndex);
                }
            }
            renderQuestions();
        }
        
        function updateQuestionText(qIndex, val) { quizData.Questions[qIndex].QuestionText = val; }
        function updateQuestionMarks(qIndex, val) { quizData.Questions[qIndex].Marks = parseInt(val) || 1; }
        function updateOptionText(qIndex, oIndex, val) { quizData.Questions[qIndex].Options[oIndex].OptionText = val; }
        
        function renderQuestions() {
            var html = '';
            for (var i=0; i<quizData.Questions.length; i++) {
                var q = quizData.Questions[i];
                html += '<div class="admin-panel" style="margin-bottom:20px; border-left:4px solid var(--primary); padding-left:15px;">';
                html += '<div style="float:right;"><button type="button" class="btn btn-danger btn-small" onclick="deleteQuestion('+i+')">Delete</button></div>';
                html += '<h4>Question ' + (i+1) + '</h4>';
                
                html += '<div class="form-field"><label>Question Text</label><textarea class="form-control" rows="2" onchange="updateQuestionText('+i+', this.value)">'+(q.QuestionText||'')+'</textarea></div>';
                
                html += '<div style="display:flex; gap:20px;">';
                html += '<div class="form-field" style="flex:1;"><label>Question Type</label><select class="form-control" onchange="changeQuestionType('+i+', this.value)">';
                html += '<option value="SingleChoice" '+(q.QuestionType==='SingleChoice'?'selected':'')+'>Multiple Choice (1 correct)</option>';
                html += '<option value="TrueFalse" '+(q.QuestionType==='TrueFalse'?'selected':'')+'>True / False</option>';
                html += '<option value="SingleAnswer" '+(q.QuestionType==='SingleAnswer'?'selected':'')+'>Single Answer (Fill in blank)</option>';
                html += '</select></div>';
                html += '<div class="form-field" style="width:100px;"><label>Points</label><input type="number" class="form-control" value="'+q.Marks+'" onchange="updateQuestionMarks('+i+', this.value)" /></div>';
                html += '</div>';
                
                html += '<div style="background:#f8f9fa; padding:15px; border-radius:4px; margin-top:10px;">';
                if (q.QuestionType === 'SingleChoice') {
                    html += '<label>Options</label>';
                    for (var j=0; j<q.Options.length; j++) {
                        var o = q.Options[j];
                        html += '<div style="display:flex; gap:10px; margin-bottom:10px; align-items:center;">';
                        html += '<input type="radio" name="correct_'+i+'" '+(o.IsCorrect?'checked':'')+' onchange="setCorrectOption('+i+', '+j+')" title="Mark as correct" />';
                        html += '<input type="text" class="form-control" style="flex:1;" value="'+(o.OptionText||'')+'" onchange="updateOptionText('+i+', '+j+', this.value)" placeholder="Option text" />';
                        if (q.Options.length > 2) {
                            html += '<button type="button" class="btn btn-outline btn-small" onclick="deleteOption('+i+', '+j+')">X</button>';
                        }
                        html += '</div>';
                    }
                    html += '<button type="button" class="btn btn-outline btn-small" onclick="addOption('+i+')">+ Add Option</button>';
                } 
                else if (q.QuestionType === 'TrueFalse') {
                    html += '<label>Options</label>';
                    for (var j=0; j<2; j++) {
                        var o = q.Options[j];
                        html += '<div style="display:flex; gap:10px; margin-bottom:10px; align-items:center;">';
                        html += '<input type="radio" name="correct_'+i+'" '+(o.IsCorrect?'checked':'')+' onchange="setCorrectOption('+i+', '+j+')" />';
                        html += '<span style="flex:1;">'+o.OptionText+'</span>';
                        html += '</div>';
                    }
                }
                else if (q.QuestionType === 'SingleAnswer') {
                    var o = q.Options[0] || { OptionText: '' };
                    html += '<div class="form-field"><label>Correct Answer (Exact match required)</label>';
                    html += '<input type="text" class="form-control" value="'+(o.OptionText||'')+'" onchange="updateOptionText('+i+', 0, this.value)" /></div>';
                }
                html += '</div>';
                html += '</div>';
            }
            document.getElementById('questionsContainer').innerHTML = html;
        }
        
        function saveQuizData() {
            quizData.Title = document.getElementById('txtQuizTitle').value;
            quizData.Description = document.getElementById('txtQuizDesc').value;
            quizData.TimeLimitMinutes = parseInt(document.getElementById('txtTimeLimit').value) || null;
            quizData.PassingScore = parseInt(document.getElementById('txtPassingScore').value) || 50;
            quizData.IsPublished = document.getElementById('chkIsPublished').checked;
            
            if (!quizData.Title) {
                alert('Please enter a Quiz Title.');
                return false;
            }
            
            // Re-render hidden field
            document.getElementById('<%= hfQuizData.ClientID %>').value = JSON.stringify(quizData);
            return true;
        }
    </script>
</asp:Content>
