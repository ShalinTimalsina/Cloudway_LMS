using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Techspire_LMS.BLL;
using Techspire_LMS.Models;

namespace Techspire_LMS.Admin
{
    public partial class ManagaeQuiz : System.Web.UI.Page
    {
        private readonly QuizBLL _bll = new QuizBLL();

        private int SelectedCourseId
        {
            get { return string.IsNullOrEmpty(ddlCourse.SelectedValue) ? 0 : int.Parse(ddlCourse.SelectedValue); }
        }
        private int CurrentQuizId
        {
            get { return int.Parse(hfQuizId.Value); }
            set { hfQuizId.Value = value.ToString(); }
        }
        private int CurrentQuestionId
        {
            get { return int.Parse(hfQuestionId.Value); }
            set { hfQuestionId.Value = value.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourseDropdown();
            }
        }

        private void BindCourseDropdown()
        {
            ddlCourse.Items.Clear();
            ddlCourse.Items.Add(new ListItem("— Select a course —", ""));
            foreach (var c in new CourseBLL().GetAllForAdmin())
                ddlCourse.Items.Add(new ListItem(c.Title, c.CourseID.ToString()));
        }

        // ====================================================================
        // LEVEL 1 — Course selection -> Quiz list
        // ====================================================================
        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentQuizId = 0;
            CurrentQuestionId = 0;
            phQuestionPanel.Visible = false;
            phOptionPanel.Visible = false;

            phQuizPanel.Visible = SelectedCourseId > 0;
            ResetQuizForm();
            if (SelectedCourseId > 0) BindQuizzesGrid();
        }

        private void BindQuizzesGrid()
        {
            gvQuizzes.DataSource = _bll.GetByCourse(SelectedCourseId);
            gvQuizzes.DataBind();
        }

        protected void btnSaveQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                Quiz q = new Quiz
                {
                    QuizID = CurrentQuizId,
                    CourseID = SelectedCourseId,
                    Title = txtQuizTitle.Text,
                    PassMark = string.IsNullOrWhiteSpace(txtPassMark.Text) ? 50 : int.Parse(txtPassMark.Text),
                    IsActive = chkQuizActive.Checked
                };

                if (q.QuizID == 0) { _bll.AddQuiz(q); litMessage.Text = Success("Quiz added."); }
                else { _bll.EditQuiz(q); litMessage.Text = Success("Quiz updated."); }

                ResetQuizForm();
                BindQuizzesGrid();
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }
        }

        protected void gvQuizzes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int quizId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                Quiz q = _bll.GetById(quizId);
                if (q == null) return;

                CurrentQuizId = q.QuizID;
                txtQuizTitle.Text = q.Title;
                txtPassMark.Text = q.PassMark.ToString();
                chkQuizActive.Checked = q.IsActive;
                litQuizFormTitle.Text = "Edit quiz";
            }
            else if (e.CommandName == "DeleteRow")
            {
                try { _bll.RemoveQuiz(quizId); litMessage.Text = Success("Quiz deleted."); }
                catch (ValidationException vex) { litMessage.Text = Error(vex.Message); }

                if (CurrentQuizId == quizId) { CurrentQuizId = 0; phQuestionPanel.Visible = false; }
                BindQuizzesGrid();
            }
            else if (e.CommandName == "ManageQuestions")
            {
                Quiz q = _bll.GetById(quizId);
                if (q == null) return;

                CurrentQuizId = quizId;
                litCurrentQuizTitle.Text = Server.HtmlEncode(q.Title);
                phQuestionPanel.Visible = true;
                phOptionPanel.Visible = false;
                ResetQuestionForm();
                BindQuestionsGrid();
            }
        }

        protected void btnCancelQuiz_Click(object sender, EventArgs e) { ResetQuizForm(); }

        private void ResetQuizForm()
        {
            CurrentQuizId = 0;
            txtQuizTitle.Text = "";
            txtPassMark.Text = "50";
            chkQuizActive.Checked = true;
            litQuizFormTitle.Text = "Add a quiz";
        }

        // ====================================================================
        // LEVEL 2 — Quiz -> Question list
        // ====================================================================
        private void BindQuestionsGrid()
        {
            gvQuestions.DataSource = _bll.GetQuestionsWithOptions(CurrentQuizId);
            gvQuestions.DataBind();
        }

        protected void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            try
            {
                Question q = new Question
                {
                    QuestionID = CurrentQuestionId,
                    QuizID = CurrentQuizId,
                    QuestionText = txtQuestionText.Text,
                    QuestionType = ddlQuestionType.SelectedValue,
                    Marks = string.IsNullOrWhiteSpace(txtMarks.Text) ? 1 : int.Parse(txtMarks.Text)
                };

                if (q.QuestionID == 0) { _bll.AddQuestion(q); litMessage.Text = Success("Question added."); }
                else { _bll.EditQuestion(q); litMessage.Text = Success("Question updated."); }

                ResetQuestionForm();
                BindQuestionsGrid();
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }
        }

        protected void gvQuestions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int questionId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                Question q = _bll.GetQuestionsWithOptions(CurrentQuizId).Find(x => x.QuestionID == questionId);
                if (q == null) return;

                CurrentQuestionId = q.QuestionID;
                txtQuestionText.Text = q.QuestionText;
                ddlQuestionType.SelectedValue = q.QuestionType;
                txtMarks.Text = q.Marks.ToString();
            }
            else if (e.CommandName == "DeleteRow")
            {
                try { _bll.RemoveQuestion(questionId); litMessage.Text = Success("Question deleted."); }
                catch (ValidationException vex) { litMessage.Text = Error(vex.Message); }

                if (CurrentQuestionId == questionId) { CurrentQuestionId = 0; phOptionPanel.Visible = false; }
                BindQuestionsGrid();
            }
            else if (e.CommandName == "ManageOptions")
            {
                var question = _bll.GetQuestionsWithOptions(CurrentQuizId).Find(x => x.QuestionID == questionId);
                if (question == null) return;

                CurrentQuestionId = questionId;
                litCurrentQuestionText.Text = Server.HtmlEncode(question.QuestionText);
                phOptionPanel.Visible = true;
                ResetOptionForm();
                BindOptionsGrid();
            }
        }

        protected void btnCancelQuestion_Click(object sender, EventArgs e) { ResetQuestionForm(); }

        protected void btnCloseQuestionPanel_Click(object sender, EventArgs e)
        {
            phQuestionPanel.Visible = false;
            phOptionPanel.Visible = false;
            CurrentQuizId = 0;
            CurrentQuestionId = 0;
        }

        private void ResetQuestionForm()
        {
            CurrentQuestionId = 0;
            txtQuestionText.Text = "";
            ddlQuestionType.SelectedValue = "SingleChoice";
            txtMarks.Text = "1";
        }

        // ====================================================================
        // LEVEL 3 — Question -> Option list
        // ====================================================================
        private void BindOptionsGrid()
        {
            var question = _bll.GetQuestionsWithOptions(CurrentQuizId).Find(x => x.QuestionID == CurrentQuestionId);
            gvOptions.DataSource = question != null ? question.Options : null;
            gvOptions.DataBind();
        }

        protected void btnSaveOption_Click(object sender, EventArgs e)
        {
            try
            {
                QuestionOption o = new QuestionOption
                {
                    OptionID = int.Parse(hfOptionEditId.Value),
                    QuestionID = CurrentQuestionId,
                    OptionText = txtOptionText.Text,
                    IsCorrect = chkIsCorrect.Checked,
                    SortOrder = string.IsNullOrWhiteSpace(txtOptionSort.Text) ? 1 : int.Parse(txtOptionSort.Text)
                };

                if (o.OptionID == 0) { _bll.AddOption(o); litMessage.Text = Success("Option added."); }
                else { _bll.EditOption(o); litMessage.Text = Success("Option updated."); }

                ResetOptionForm();
                BindOptionsGrid();
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }
        }

        protected void gvOptions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int optionId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditRow")
            {
                var question = _bll.GetQuestionsWithOptions(CurrentQuizId).Find(x => x.QuestionID == CurrentQuestionId);
                var option = question != null ? question.Options.Find(o => o.OptionID == optionId) : null;
                if (option == null) return;

                hfOptionEditId.Value = option.OptionID.ToString();
                txtOptionText.Text = option.OptionText;
                chkIsCorrect.Checked = option.IsCorrect;
                txtOptionSort.Text = option.SortOrder.ToString();
            }
            else if (e.CommandName == "DeleteRow")
            {
                try { _bll.RemoveOption(optionId); litMessage.Text = Success("Option deleted."); }
                catch (ValidationException vex) { litMessage.Text = Error(vex.Message); }
                BindOptionsGrid();
            }
        }

        protected void btnCancelOption_Click(object sender, EventArgs e) { ResetOptionForm(); }

        protected void btnCloseOptionPanel_Click(object sender, EventArgs e)
        {
            phOptionPanel.Visible = false;
            CurrentQuestionId = 0;
        }

        private void ResetOptionForm()
        {
            hfOptionEditId.Value = "0";
            txtOptionText.Text = "";
            chkIsCorrect.Checked = false;
            txtOptionSort.Text = "1";
        }

        // ====================================================================
        private string Success(string msg) { return "<div class=\"alert alert-success\">" + Server.HtmlEncode(msg) + "</div>"; }
        private new string Error(string msg) { return "<div class=\"alert alert-error\">" + Server.HtmlEncode(msg) + "</div>"; }
    
}
}