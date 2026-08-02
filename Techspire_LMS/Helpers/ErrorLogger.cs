using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace Techspire_LMS.Helpers
{
    /// <summary>
    /// Writes exceptions to a text file inside App_Data, which ASP.NET blocks
    /// from direct HTTP access — so the log cannot be downloaded by a visitor.
    /// Add &lt;add key="ErrorLogPath" value="~/App_Data/errors.log" /&gt; to
    /// Web.config's &lt;appSettings&gt;.
    /// </summary>
    public static class ErrorLogger
    {
        // Two threads writing to one file at the same instant corrupts it.
        // lock() lets exactly one thread inside at a time.
        private static readonly object _gate = new object();

        public static void Log(Exception ex, string context)
        {
            if (ex == null) return;

            try
            {
                string configuredPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                if (string.IsNullOrWhiteSpace(configuredPath))
                    configuredPath = "~/App_Data/errors.log";

                string path = HttpContext.Current != null
                    ? HttpContext.Current.Server.MapPath(configuredPath)
                    : configuredPath;

                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string entry = string.Format(
                    "{0:yyyy-MM-dd HH:mm:ss}  [{1}]{2}{3}{2}{4}{2}{2}",
                    DateTime.Now, context, Environment.NewLine, ex.ToString(), new string('-', 70));

                lock (_gate)
                {
                    File.AppendAllText(path, entry);
                }
            }
            catch
            {
                // A logger must never throw — failing to log must not replace
                // the user's original error with a new, unrelated one.
            }
        }
    }
}
