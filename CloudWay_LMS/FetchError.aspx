<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>
<!DOCTYPE html>
<html>
<body>
<%
    try {
        var client = new WebClient();
        client.DownloadString("http://localhost:8080/Default.aspx");
    } catch (WebException ex) {
        using (var reader = new StreamReader(ex.Response.GetResponseStream())) {
            string html = reader.ReadToEnd();
            File.WriteAllText(Server.MapPath("~/default_root_err.html"), html);
            Response.Write("Saved to default_root_err.html");
        }
    }
%>
</body>
</html>
