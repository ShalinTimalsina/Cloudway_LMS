using System;
using System.Collections.Generic;
using System.Linq;
using CloudWay_LMS.Data_Access_Layer;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.BLL
{
    public class QuizBLL
    {
        private readonly QuizDAL _quizDal = new QuizDAL();
        private readonly QuestionDAL _questionDal = new QuestionDAL();
        private readonly QuestionOptionDAL _optionDal = new QuestionOptionDAL();
        private readonly QuizAttemptDAL _attemptDal = new QuizAttemptDAL();

        private static readonly string[] AllowedTypes = { "SingleChoice", "MultipleChoice", "TrueFalse", "SingleAnswer" };

        // ================================================================
        // READS
        // ================================================================
        public List<Quiz> GetByCourse(int courseId) { return _quizDal.SelectByCourse(courseId); }
        public Quiz GetById(int quizId) { return quizId <= 0 ? null : _quizDal.SelectById(quizId); }

        /// <summary>What a quiz-taking page needs: every question, each with its
        /// options attached, ready to bind to a Repeater/GridView. IsCorrect is
        /// still on each option here — hide it in the UI layer for a learner
        /// taking the quiz, but an admin preview page can show it directly.</summary>
        public List<Question> GetQuestionsWithOptions(int quizId)
        {
            List<Question> questions = _questionDal.SelectByQuiz(quizId);
            foreach (Question q in questions)
                q.Options = _optionDal.SelectByQuestion(q.QuestionID);
            return questions;
        }

        public List<QuizAttempt> GetAttemptsByUser(int userId) { return _attemptDal.SelectByUser(userId); }
        public List<QuizAttempt> GetAttemptsByQuiz(int quizId) { return _attemptDal.SelectByQuiz(quizId); }

        // ================================================================
        // SCORING — the core of this class
        // ================================================================
        /// <summary>
        /// Scores a submitted attempt and stores a permanent, immutable snapshot.
        ///
        /// answers: QuestionID -> the OptionIDs the learner selected for that
        /// question. A question with no entry in `answers` counts as
        /// unanswered (zero marks for that question, not an error) — this
        /// matters for a learner who skips a question rather than guessing.
        ///
        /// GRADING RULE: a question earns full marks only when the learner's
        /// selected set EXACTLY equals the set of options flagged IsCorrect —
        /// no more, no fewer. This one rule handles SingleChoice (one correct
        /// option, exact match means "picked it"), TrueFalse (same, two
        /// options), and MultipleChoice (must select every correct option and
        /// no incorrect one) uniformly, with no per-type branching. There is
        /// no partial credit; a MultipleChoice question with 3 correct options
        /// where the learner picks 2 of them scores 0 for that question. If
        /// your assignment wants partial credit, that is a good "extension"
        /// to describe in the report — the natural place to add it is right
        /// here, per-question, before the score is summed.
        ///
        /// Score/TotalMarks/IsPassed are computed from THIS submission against
        /// the quiz's CURRENT questions and PassingScore, then stored as a
        /// snapshot. Editing the quiz later never rewrites a past result —
        /// that is why QuizAttempts has its own TotalMarks column rather than
        /// recomputing it from Questions every time it's displayed.
        /// </summary>
        public QuizAttempt SubmitAttempt(int userId, int quizId, Dictionary<int, List<int>> answers)
        {
            if (userId <= 0)
                throw new ValidationException("You must be logged in to submit a quiz.");

            Quiz quiz = _quizDal.SelectById(quizId);
            if (quiz == null)
                throw new ValidationException("This quiz is not available.");

            if (answers == null) answers = new Dictionary<int, List<int>>();

            List<Question> questions = GetQuestionsWithOptions(quizId);
            if (questions.Count == 0)
                throw new ValidationException("This quiz has no questions yet.");

            int totalMarks = 0;
            int score = 0;

            foreach (Question q in questions)
            {
                totalMarks += q.Marks;

                HashSet<int> correctOptionIds = new HashSet<int>(
                    q.Options.Where(o => o.IsCorrect).Select(o => o.OptionID));

                List<int> submitted;
                HashSet<int> selectedOptionIds = answers.TryGetValue(q.QuestionID, out submitted)
                    ? new HashSet<int>(submitted)
                    : new HashSet<int>();

                if (correctOptionIds.SetEquals(selectedOptionIds))
                    score += q.Marks;
            }

            bool isPassed = totalMarks > 0
                && ((decimal)score * 100m / totalMarks) >= quiz.PassingScore;

            QuizAttempt attempt = new QuizAttempt
            {
                UserID = userId,
                QuizID = quizId,
                Score = score,
                TotalMarks = totalMarks,
                IsPassed = isPassed
            };

            attempt.AttemptID = _attemptDal.Insert(attempt);
            return attempt;
        }

        // ================================================================
        // ADMIN AUTHORING — quiz / question / option CRUD with validation
        // ================================================================
        public int AddQuiz(Quiz q)
        {
            ValidateQuiz(q);
            return _quizDal.Insert(q);
        }

        public void EditQuiz(Quiz q)
        {
            if (q == null || q.QuizID <= 0) throw new ValidationException("Invalid quiz.");
            ValidateQuiz(q);
            if (!_quizDal.Update(q)) throw new ValidationException("The quiz no longer exists.");
        }

        public void RemoveQuiz(int quizId)
        {
            if (quizId <= 0) throw new ValidationException("Invalid quiz.");

            if (_quizDal.HasAttempts(quizId))
                throw new ValidationException("This quiz has been attempted by learners and cannot be deleted. Deactivate it instead (uncheck 'Active').");

            if (!_quizDal.Delete(quizId)) throw new ValidationException("The quiz no longer exists.");
        }

        public int AddQuestion(Question q)
        {
            ValidateQuestion(q);
            return _questionDal.Insert(q);
        }

        public void EditQuestion(Question q)
        {
            if (q == null || q.QuestionID <= 0) throw new ValidationException("Invalid question.");
            ValidateQuestion(q);
            if (!_questionDal.Update(q)) throw new ValidationException("The question no longer exists.");
        }

        public void RemoveQuestion(int questionId)
        {
            if (questionId <= 0) throw new ValidationException("Invalid question.");
            if (!_questionDal.Delete(questionId)) throw new ValidationException("The question no longer exists.");
        }

        public int AddOption(QuestionOption o)
        {
            ValidateOption(o);
            return _optionDal.Insert(o);
        }

        public void EditOption(QuestionOption o)
        {
            if (o == null || o.OptionID <= 0) throw new ValidationException("Invalid option.");
            ValidateOption(o);
            if (!_optionDal.Update(o)) throw new ValidationException("The option no longer exists.");
        }

        public void RemoveOption(int optionId)
        {
            if (optionId <= 0) throw new ValidationException("Invalid option.");
            if (!_optionDal.Delete(optionId)) throw new ValidationException("The option no longer exists.");
        }

        // ---------------- Validation ----------------

        private void ValidateQuiz(Quiz q)
        {
            if (q == null) throw new ValidationException("No quiz data was supplied.");
            if (string.IsNullOrWhiteSpace(q.Title)) throw new ValidationException("Quiz title is required.");
            q.Title = q.Title.Trim();
            if (q.Title.Length > 150) throw new ValidationException("Quiz title must be 150 characters or fewer.");
            if (q.CourseID <= 0) throw new ValidationException("Quiz must belong to a course.");
            if (q.PassingScore < 0 || q.PassingScore > 100) throw new ValidationException("Pass mark must be between 0 and 100.");
            if (!string.IsNullOrEmpty(q.Description) && q.Description.Length > 1000) throw new ValidationException("Description must be 1000 characters or fewer.");
            if (q.TimeLimitMinutes.HasValue && q.TimeLimitMinutes.Value <= 0) throw new ValidationException("Time limit must be greater than zero.");
        }

        private void ValidateQuestion(Question q)
        {
            if (q == null) throw new ValidationException("No question data was supplied.");
            if (q.QuizID <= 0) throw new ValidationException("Question must belong to a quiz.");
            if (string.IsNullOrWhiteSpace(q.QuestionText)) throw new ValidationException("Question text is required.");
            q.QuestionText = q.QuestionText.Trim();
            if (q.QuestionText.Length > 500) throw new ValidationException("Question text must be 500 characters or fewer.");
            if (Array.IndexOf(AllowedTypes, q.QuestionType) < 0)
                throw new ValidationException("Question type must be SingleChoice, MultipleChoice, TrueFalse or SingleAnswer.");
            if (q.Marks <= 0) throw new ValidationException("Marks must be greater than zero.");
        }

        private void ValidateOption(QuestionOption o)
        {
            if (o == null) throw new ValidationException("No option data was supplied.");
            if (o.QuestionID <= 0) throw new ValidationException("Option must belong to a question.");
            if (string.IsNullOrWhiteSpace(o.OptionText)) throw new ValidationException("Option text is required.");
            o.OptionText = o.OptionText.Trim();
            if (o.OptionText.Length > 250) throw new ValidationException("Option text must be 250 characters or fewer.");
        }
    }
}
