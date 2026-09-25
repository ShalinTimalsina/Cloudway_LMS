<%@ Page Language="C#" %>
<%@ Import Namespace="CloudWay_LMS.BLL" %>
<!DOCTYPE html>
<html>
<body>
<%
    try {
        var user = new AuthBLL().Login("student@cloudway.local", "Student123!");
        Response.Write("SUCCESS! User ID: " + user.UserID + ", Role: " + user.RoleID);
    } catch (Exception ex) {
        Response.Write("FAILED: " + ex.Message);
    }
%>
</body>
</html>
