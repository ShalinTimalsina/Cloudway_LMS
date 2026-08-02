using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Techspire_LMS.Data_Access_Layer
{
    /// <summary>
    /// Central place for everything ADO.NET-plumbing related.
    /// No business rules here — this class only knows how to talk to SQL Server.
    /// Add a connection string named "LearningPlatformDB" to Web.config's
    /// &lt;connectionStrings&gt; section before using anything in App_Code.
    /// </summary>
    public static class DbHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["LearningPlatformDB"].ConnectionString;

        /// <summary>Creates (but does NOT open) a connection. Wrap the caller in using.</summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>Builds a command bound to a connection.</summary>
        public static SqlCommand CreateCommand(
            SqlConnection con, string sql, CommandType type = CommandType.Text)
        {
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = type;
            cmd.CommandTimeout = 30;
            return cmd;
        }

        /// <summary>Adds a parameter, translating C# null into database NULL.</summary>
        public static void AddParam(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        /// <summary>Typed parameter version — keeps execution plans stable for strings.</summary>
        public static void AddParam(SqlCommand cmd, string name,
                                    SqlDbType type, int size, object value)
        {
            SqlParameter p = new SqlParameter(name, type, size);
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        // ---------- Safe readers: turn database NULL into a usable C# value ----------

        public static string GetString(IDataRecord r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        public static int GetInt(IDataRecord r, string col, int fallback = 0)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? fallback : r.GetInt32(i);
        }

        public static int? GetNullableInt(IDataRecord r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (int?)null : r.GetInt32(i);
        }

        public static bool GetBool(IDataRecord r, string col, bool fallback = false)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? fallback : r.GetBoolean(i);
        }

        public static decimal GetDecimal(IDataRecord r, string col, decimal fallback = 0m)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? fallback : r.GetDecimal(i);
        }

        public static DateTime GetDate(IDataRecord r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? DateTime.MinValue : r.GetDateTime(i);
        }

        public static DateTime? GetNullableDate(IDataRecord r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (DateTime?)null : r.GetDateTime(i);
        }
    }
}
