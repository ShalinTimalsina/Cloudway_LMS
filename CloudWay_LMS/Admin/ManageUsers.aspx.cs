using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudWay_LMS.BLL;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.Admin
{
    public partial class ManageUsers : System.Web.UI.Page
    {

        private readonly UserBLL _bll = new UserBLL();

        // Roles seeded as 1 = Admin, 2 = Member (see CreateDatabase.sql).
        // Hard-coding these two IDs here keeps the toggle button a single
        // click instead of a dropdown + save — reasonable for a two-role
        // template; swap to a RoleBLL-backed dropdown if you add more roles.
        private const int AdminRoleId = 1;
        private const int MemberRoleId = 2;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvUsers.DataSource = _bll.GetAll();
            gvUsers.DataBind();
        }

        /// <summary>Sets each row's button labels/visibility based on that row's
        /// own data — can't be done in markup since it depends on both the
        /// row's values AND who's currently logged in.</summary>
        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            User u = (User)e.Row.DataItem;
            LinkButton lnkRole = (LinkButton)e.Row.FindControl("lnkRole");
            LinkButton lnkStatus = (LinkButton)e.Row.FindControl("lnkStatus");
            LinkButton lnkUnlock = (LinkButton)e.Row.FindControl("lnkUnlock");
            Literal litLocked = (Literal)e.Row.FindControl("litLocked");

            bool isLocked = u.LockoutEndUtc.HasValue && u.LockoutEndUtc.Value > DateTime.UtcNow;
            litLocked.Text = isLocked ? " <span class=\"badge badge-draft\">Locked</span>" : "";
            lnkUnlock.Visible = isLocked;

            bool isSelf = u.UserID == AuthBLL.CurrentUserId;

            if (isSelf)
            {
                // Defence in depth: UserBLL also rejects this server-side,
                // but hiding the buttons for your own row stops the confusing
                // "why did that fail?" click in the first place.
                lnkRole.Visible = false;
                lnkStatus.Visible = false;
                return;
            }

            bool isAdmin = u.RoleID == AdminRoleId;
            lnkRole.Text = isAdmin ? "Demote to Member" : "Promote to Admin";
            lnkRole.Attributes["onclick"] = "return confirm('" +
                (isAdmin ? "Remove admin access for this user?" : "Grant admin access to this user?") + "');";

            lnkStatus.Text = u.IsActive ? "Deactivate" : "Reactivate";
            lnkStatus.CssClass += u.IsActive ? " btn-danger" : "";
            if (u.IsActive)
                lnkStatus.Attributes["onclick"] = "return confirm('Deactivate this account? They will no longer be able to log in.');";
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId = int.Parse((string)e.CommandArgument);

            try
            {
                if (e.CommandName == "ToggleRole")
                {
                    User u = _bll.GetById(userId);
                    if (u == null) throw new ValidationException("User not found.");

                    int newRoleId = u.RoleID == AdminRoleId ? MemberRoleId : AdminRoleId;
                    _bll.ChangeRole(userId, newRoleId, AuthBLL.CurrentUserId);
                    litMessage.Text = Success("Role updated.");
                }
                else if (e.CommandName == "ToggleStatus")
                {
                    User u = _bll.GetById(userId);
                    if (u == null) throw new ValidationException("User not found.");

                    if (u.IsActive) _bll.Deactivate(userId, AuthBLL.CurrentUserId);
                    else _bll.Reactivate(userId);
                    litMessage.Text = Success("Status updated.");
                }
                else if (e.CommandName == "ClearLockout")
                {
                    _bll.ClearLockout(userId);
                    litMessage.Text = Success("Lockout cleared.");
                }
            }
            catch (ValidationException vex)
            {
                litMessage.Text = Error(vex.Message);
            }

            BindGrid();
        }

        private string Success(string msg) { return "<div class=\"alert alert-success\">" + Server.HtmlEncode(msg) + "</div>"; }
        private string Error(string msg) { return "<div class=\"alert alert-error\">" + Server.HtmlEncode(msg) + "</div>"; }

        protected void gvUsers_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvUsers.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
    }