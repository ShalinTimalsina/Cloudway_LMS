using System.Data.SqlClient;
using CloudWay_LMS.BLL;

namespace CloudWay_LMS.Helpers
{
    /// <summary>
    /// Turns a raw SqlException into a friendly ValidationException, or returns
    /// null when the error isn't one of the recognised constraint violations —
    /// in which case the caller should log it and show a generic message.
    ///
    /// Usage in every BLL write method:
    /// <code>
    /// catch (SqlException sqlEx)
    /// {
    ///     var vex = SqlErrorHelper.Translate(sqlEx, "That email is already registered.");
    ///     if (vex != null) throw vex;
    ///     ErrorLogger.Log(sqlEx, "UserBLL.Register");
    ///     throw new ValidationException("A database error occurred. Please try again.");
    /// }
    /// </code>
    /// </summary>
    public static class SqlErrorHelper
    {
        private const int UniqueViolation1 = 2627;  // PRIMARY KEY / UNIQUE constraint
        private const int UniqueViolation2 = 2601;  // duplicate key on a unique index
        private const int ForeignKeyViolation = 547; // FK constraint (insert/update OR delete)

        public static ValidationException Translate(
            SqlException ex, string uniqueMessage = null, string fkMessage = null)
        {
            if (ex.Number == UniqueViolation1 || ex.Number == UniqueViolation2)
                return new ValidationException(uniqueMessage ?? "That value is already in use.");

            if (ex.Number == ForeignKeyViolation)
                return new ValidationException(fkMessage ??
                    "This action conflicts with related data and cannot be completed.");

            return null; // not a recognised constraint violation — caller logs and shows a generic message
        }
    }
}
