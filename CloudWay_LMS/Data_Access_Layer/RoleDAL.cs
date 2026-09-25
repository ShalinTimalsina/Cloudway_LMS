using System.Collections.Generic;
using System.Data.SqlClient;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Data_Access_Layer
{
    public class RoleDAL
    {
        public List<Role> SelectAll()
        {
            List<Role> list = new List<Role>();
            const string sql = "SELECT RoleID, RoleName, Description FROM Roles ORDER BY RoleName;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }
            return list;
        }

        public Role SelectById(int roleId)
        {
            const string sql = "SELECT RoleID, RoleName, Description FROM Roles WHERE RoleID = @RoleID;";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = DbHelper.CreateCommand(con, sql))
            {
                DbHelper.AddParam(cmd, "@RoleID", roleId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    return r.Read() ? Map(r) : null;
                }
            }
        }

        private Role Map(SqlDataReader r)
        {
            return new Role
            {
                RoleID      = DbHelper.GetInt(r, "RoleID"),
                RoleName    = DbHelper.GetString(r, "RoleName"),
                Description = DbHelper.GetString(r, "Description")
            };
        }
    }
}
