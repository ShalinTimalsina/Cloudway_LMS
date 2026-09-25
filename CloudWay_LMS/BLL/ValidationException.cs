using System;

namespace CloudWay_LMS.BLL
{
    /// <summary>
    /// Carries a message that is SAFE to show directly to a user. Distinguishing
    /// "the user did something wrong" (ValidationException) from "the system
    /// broke" (any other Exception) lets a page show one and log the other:
    ///
    ///   catch (ValidationException vex) { ShowError(vex.Message); }
    ///   catch (Exception ex) { ErrorLogger.Log(ex, "context"); ShowError("Something went wrong."); }
    ///
    /// catch blocks run in order, so the specific type must be caught BEFORE
    /// the general one.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}
