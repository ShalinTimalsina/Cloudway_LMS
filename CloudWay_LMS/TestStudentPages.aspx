<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>
<!DOCTYPE html>
<html>
<body>
<%
    string[] pages = {
        "Default.aspx",
        "Pages/Courses.aspx",
        "Pages/CourseDetails.aspx?id=1",
        "Pages/LessonDetails.aspx?id=1",
        "Pages/Contact.aspx",
        "Account/Login.aspx",
        "Account/Register.aspx",
        "Member/MyLearning.aspx",
        "Member/Dashboard.aspx",
        "Member/Quiz.aspx?id=1"
    };

    foreach (var p in pages) {
        try {
            var req = WebRequest.Create("http://localhost:8080/" + p);
            var res = (HttpWebResponse)req.GetResponse();
            Response.Write("<p>" + p + ": " + res.StatusCode + "</p>");
        } catch (WebException ex) {
            if (ex.Response != null) {
                var res = (HttpWebResponse)ex.Response;
                Response.Write("<p>" + p + ": ERROR " + res.StatusCode + "</p>");
                using (var reader = new StreamReader(ex.Response.GetResponseStream())) {
                    string html = reader.ReadToEnd();
                    int idx = html.IndexOf("Exception Details:");
                    if (idx != -1) {
                        Response.Write("<pre>" + Server.HtmlEncode(html.Substring(idx, 500)) + "</pre>");
                    }
                }
            } else {
                Response.Write("<p>" + p + ": " + ex.Message + "</p>");
            }
        }
    }
%>
</body>
</html>
