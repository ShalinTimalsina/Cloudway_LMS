using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Member
{
    public partial class Quiz : System.Web.UI.Page
    {

        private int QuizId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Page-level guard, same idea as Admin.master's check but for any
            // single member-only page not using a dedicated master. Identity for
            // grading always comes from AuthBLL.CurrentUserId (Session), never
            // from anything in the URL.
            if (!AuthBLL.IsLoggedIn)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
                LoadQuiz();
        }

        private void LoadQuiz()
        {
            CloudWay_LMS.Models.Quiz quiz = new QuizBLL().GetById(QuizId);
            if (quiz == null)
            {
                phNotFound.Visible = true;
                phQuizForm.Visible = false;
                return;
            }

            litQuizTitle.Text = Server.HtmlEncode(quiz.Title);
            litPassMark.Text = quiz.PassingScore.ToString();

            List<Question> questions = new QuizBLL().GetQuestionsWithOptions(QuizId);
            rptQuestions.DataSource = questions;
            rptQuestions.DataBind();
        }

        /// <summary>Fills each question's CheckBoxList from ITS OWN Options list —
        /// this is why the outer bind uses ItemDataBound rather than plain markup:
        /// each row needs a second, independent DataBind call.</summary>
        protected void rptQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            Question q = (Question)e.Item.DataItem;
            CheckBoxList cbl = (CheckBoxList)e.Item.FindControl("cblOptions");

            cbl.DataSource = q.Options;
            cbl.DataTextField = "OptionText";
            cbl.DataValueField = "OptionID";
            cbl.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Rebuild QuestionID -> selected OptionIDs from the posted form — this
            // is the exact shape QuizBLL.SubmitAttempt expects (see its XML doc).
            Dictionary<int, List<int>> answers = new Dictionary<int, List<int>>();

            foreach (RepeaterItem item in rptQuestions.Items)
            {
                HiddenField hfQuestionId = (HiddenField)item.FindControl("hfQuestionId");
                CheckBoxList cbl = (CheckBoxList)item.FindControl("cblOptions");

                int questionId = int.Parse(hfQuestionId.Value);
                List<int> selected = new List<int>();
                foreach (ListItem li in cbl.Items)
                    if (li.Selected) selected.Add(int.Parse(li.Value));

                answers[questionId] = selected;
            }

            try
            {
                QuizAttempt result = new QuizBLL().SubmitAttempt(AuthBLL.CurrentUserId, QuizId, answers);

                phQuizForm.Visible = false;
                phResult.Visible = true;
                litScore.Text = result.Score + " / " + result.TotalMarks;
                litOutcome.Text = result.IsPassed
                    ? "<span style=\"color:var(--success)\">You passed!</span>"
                    : "<span style=\"color:var(--danger)\">Not a pass this time — you can try again.</span>";
            }
            catch (ValidationException vex)
            {
                litMessage.Text = "<div class=\"alert alert-error\">" + Server.HtmlEncode(vex.Message) + "</div>";
            }
        }
    }
}
