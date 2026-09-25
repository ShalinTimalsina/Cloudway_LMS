using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Data_Access_Layer;
using CloudWay_LMS.Models;
using Newtonsoft.Json;

namespace CloudWay_LMS.Admin
{
    public partial class ManagaeQuiz : System.Web.UI.Page
    {
        private readonly QuizBLL _quizBll = new QuizBLL();
        private readonly CourseBLL _courseBll = new CourseBLL();

        public class QuizData
        {
            public int QuizID { get; set; }
            public int CourseID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int? TimeLimitMinutes { get; set; }
            public int PassingScore { get; set; }
            public bool IsPublished { get; set; }
            public List<QuestionData> Questions { get; set; }
        }

        public class QuestionData
        {
            public int QuestionID { get; set; }
            public string QuestionText { get; set; }
            public string QuestionType { get; set; }
            public int Marks { get; set; }
            public List<OptionData> Options { get; set; }
        }

        public class OptionData
        {
            public int OptionID { get; set; }
            public string OptionText { get; set; }
            public bool IsCorrect { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RunSchemaMigration();
                BindCourseDropdown();
            }
        }

        private void RunSchemaMigration()
        {
            try
            {
                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    new SqlCommand("ALTER TABLE Quizzes ADD Description NVARCHAR(MAX) NULL;", con).ExecuteNonQuery();
                    new SqlCommand("ALTER TABLE Quizzes ADD TimeLimitMinutes INT NULL;", con).ExecuteNonQuery();
                    new SqlCommand("ALTER TABLE Quizzes ADD IsPublished BIT NOT NULL DEFAULT 0;", con).ExecuteNonQuery();
                }
            }
            catch { /* columns already exist */ }
        }

        private void BindCourseDropdown()
        {
            ddlCourse.Items.Clear();
            ddlCourse.Items.Add(new ListItem("— Select a course —", ""));
            foreach (var c in _courseBll.GetAllForAdmin())
                ddlCourse.Items.Add(new ListItem(c.Title, c.CourseID.ToString()));
        }

        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCourse.SelectedValue))
            {
                divQuizList.Visible = false;
                quizEditorPanel.Visible = false;
            }
            else
            {
                divQuizList.Visible = true;
                quizEditorPanel.Visible = false;
                BindQuizzesGrid();
            }
        }

        private void BindQuizzesGrid()
        {
            int courseId = int.Parse(ddlCourse.SelectedValue);
            gvQuizzes.DataSource = _quizBll.GetByCourse(courseId);
            gvQuizzes.DataBind();
        }

        protected void btnCreateQuiz_Click(object sender, EventArgs e)
        {
            int courseId = int.Parse(ddlCourse.SelectedValue);
            quizEditorPanel.Visible = true;
            divQuizList.Visible = false;

            var emptyData = new QuizData { CourseID = courseId, PassingScore = 50, Questions = new List<QuestionData>() };
            ClientScript.RegisterStartupScript(this.GetType(), "InitQuiz", "initEditor('" + JsonConvert.SerializeObject(emptyData).Replace("'", "\\'") + "');", true);
        }

        protected void gvQuizzes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int quizId = int.Parse((string)e.CommandArgument);

            if (e.CommandName == "EditQuiz")
            {
                Quiz q = _quizBll.GetById(quizId);
                List<Question> questions = _quizBll.GetQuestionsWithOptions(quizId);

                var data = new QuizData
                {
                    QuizID = q.QuizID,
                    CourseID = q.CourseID,
                    Title = q.Title,
                    Description = q.Description,
                    TimeLimitMinutes = q.TimeLimitMinutes,
                    PassingScore = q.PassingScore,
                    IsPublished = q.IsPublished,
                    Questions = new List<QuestionData>()
                };

                foreach (var question in questions)
                {
                    var qd = new QuestionData
                    {
                        QuestionID = question.QuestionID,
                        QuestionText = question.QuestionText,
                        QuestionType = question.QuestionType,
                        Marks = question.Marks,
                        Options = new List<OptionData>()
                    };
                    foreach (var opt in question.Options)
                    {
                        qd.Options.Add(new OptionData { OptionID = opt.OptionID, OptionText = opt.OptionText, IsCorrect = opt.IsCorrect });
                    }
                    data.Questions.Add(qd);
                }

                quizEditorPanel.Visible = true;
                divQuizList.Visible = false;

                string json = JsonConvert.SerializeObject(data);
                ClientScript.RegisterStartupScript(this.GetType(), "InitQuiz", "initEditor('" + json.Replace("'", "\\'") + "');", true);
            }
            else if (e.CommandName == "DeleteQuiz")
            {
                try
                {
                    _quizBll.RemoveQuiz(quizId);
                    litMessage.Text = "<div class=\"alert alert-success\">Quiz deleted.</div>";
                }
                catch (ValidationException vex)
                {
                    litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
                }
                BindQuizzesGrid();
            }
        }

        protected void btnSaveQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                string json = hfQuizData.Value;
                if (string.IsNullOrEmpty(json)) return;

                QuizData data = JsonConvert.DeserializeObject<QuizData>(json);

                Quiz q = new Quiz
                {
                    QuizID = data.QuizID,
                    CourseID = data.CourseID,
                    Title = data.Title,
                    Description = data.Description,
                    TimeLimitMinutes = data.TimeLimitMinutes,
                    PassingScore = data.PassingScore,
                    IsPublished = data.IsPublished
                };

                if (q.QuizID == 0)
                {
                    q.QuizID = _quizBll.AddQuiz(q);
                }
                else
                {
                    _quizBll.EditQuiz(q);
                    
                    // Bruteforce sync for prototype: Delete all old questions and re-insert them.
                    // This is much simpler than diffing for a prototype editor.
                    List<Question> oldQuestions = _quizBll.GetQuestionsWithOptions(q.QuizID);
                    foreach (var oldQ in oldQuestions)
                    {
                        _quizBll.RemoveQuestion(oldQ.QuestionID);
                    }
                }

                // Insert all new questions and options
                foreach (var qd in data.Questions)
                {
                    Question newQ = new Question
                    {
                        QuizID = q.QuizID,
                        QuestionText = qd.QuestionText,
                        QuestionType = qd.QuestionType,
                        Marks = qd.Marks
                    };
                    newQ.QuestionID = _quizBll.AddQuestion(newQ);

                    foreach (var od in qd.Options)
                    {
                        QuestionOption newO = new QuestionOption
                        {
                            QuestionID = newQ.QuestionID,
                            OptionText = od.OptionText,
                            IsCorrect = od.IsCorrect
                        };
                        _quizBll.AddOption(newO);
                    }
                }

                litMessage.Text = "<div class=\"alert alert-success\">Quiz saved successfully.</div>";
                
                quizEditorPanel.Visible = false;
                divQuizList.Visible = true;
                BindQuizzesGrid();
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(ex.Message) + "</div>";
                
                // Keep editor open if error occurs
                quizEditorPanel.Visible = true;
                divQuizList.Visible = false;
                ClientScript.RegisterStartupScript(this.GetType(), "InitQuiz", "initEditor('" + hfQuizData.Value.Replace("'", "\\'") + "');", true);
            }
        }

        protected void btnCancelQuiz_Click(object sender, EventArgs e)
        {
            quizEditorPanel.Visible = false;
            divQuizList.Visible = true;
            BindQuizzesGrid();
        }
    }
}
