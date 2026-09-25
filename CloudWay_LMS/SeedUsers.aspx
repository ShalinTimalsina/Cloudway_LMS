<%@ Page Language="C#" %>
<%@ Import Namespace="CloudWay_LMS.BLL" %>
<%@ Import Namespace="CloudWay_LMS.Data_Access_Layer" %>
<%@ Import Namespace="CloudWay_LMS.Models" %>
<%@ Import Namespace="CloudWay_LMS.Helpers" %>
<!DOCTYPE html>
<html>
<body>
<%
    try {
        var _userDal = new UserDAL();
        
        // Ensure Admin
        var admin = _userDal.SelectByEmail("admin@cloudway.local");
        if (admin == null) {
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.Hash("Admin123!", salt);
            _userDal.Insert(new User {
                FullName = "Admin Account",
                Email = "admin@cloudway.local",
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleID = 1
            });
            Response.Write("<p>Admin created.</p>");
        } else {
            Response.Write("<p>Admin already exists.</p>");
        }

        // Ensure Student
        var student = _userDal.SelectByEmail("student@cloudway.local");
        if (student == null) {
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.Hash("Student123!", salt);
            _userDal.Insert(new User {
                FullName = "Student Account",
                Email = "student@cloudway.local",
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleID = 2
            });
            Response.Write("<p>Student created.</p>");
        } else {
            Response.Write("<p>Student already exists.</p>");
        }
    } catch (Exception ex) {
        Response.Write("<pre>" + Server.HtmlEncode(ex.ToString()) + "</pre>");
    }
%>
</body>
</html>
