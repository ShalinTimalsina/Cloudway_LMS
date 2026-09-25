using System.Collections.Generic;

namespace CloudWay_LMS.Models
{
    /// <summary>
    /// QuestionType: "SingleChoice" | "MultipleChoice" | "TrueFalse".
    /// Options are NOT fixed A–D columns — see QuestionOption — so a quiz can
    /// mix single-answer and multi-answer questions freely.
    /// </summary>
    public class Question
    {
        public int    QuestionID   { get; set; }
        public int    QuizID       { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int    Marks        { get; set; }

        // Loaded separately via QuestionOptionDAL.SelectByQuestion
        public List<QuestionOption> Options { get; set; }
    }
}
