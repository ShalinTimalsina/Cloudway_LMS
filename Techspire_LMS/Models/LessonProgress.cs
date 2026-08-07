using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Techspire_LMS.Models
{ /// <summary>One row = one lesson completed within one enrollment. Not
  /// shown directly in any UI — LessonDetails.aspx and MyLearning.aspx read
  /// aggregates of this (counts, "is this lesson done") via EnrollmentBLL,
  /// never this table's rows one at a time.</summary>
    public class LessonProgress
    {
        public int LessonProgressID { get; set; }
        public int EnrollmentID { get; set; }
        public int LessonID { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}