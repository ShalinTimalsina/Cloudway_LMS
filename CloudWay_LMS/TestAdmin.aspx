<%@ Page Language="C#" %>
<%@ Import Namespace="CloudWay_LMS.BLL" %>
<%@ Import Namespace="System.Diagnostics" %>
<!DOCTYPE html>
<html>
<body>
<%
    try {
        var userBll = new UserBLL();
        var users = userBll.GetAll();
        Response.Write("<p>Users loaded: " + users.Count + "</p>");
    } catch (Exception ex) {
        Response.Write("<p>Users Error: " + ex.Message + "<br/>" + ex.StackTrace + "</p>");
    }

    try {
        var courseBll = new CourseBLL();
        var courses = courseBll.GetAllForAdmin();
        Response.Write("<p>Courses loaded: " + courses.Count + "</p>");
    } catch (Exception ex) {
        Response.Write("<p>Courses Error: " + ex.Message + "<br/>" + ex.StackTrace + "</p>");
    }
%>
</body>
</html>
