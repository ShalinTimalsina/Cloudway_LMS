using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CloudWay_LMS.Data_Access_Layer;
using CloudWay_LMS.Models;

namespace CloudWay_LMS.BLL
{
    public class UserBLL : PersistentConnection
    {
        private readonly UserDAL _dal = new UserDAL();

        public List<User> GetAll() { return _dal.SelectAll(); }
        public User GetById(int userId) { return userId <= 0 ? null : _dal.SelectById(userId); }

        /// <summary>Self-service profile edit — the account owner changing
        /// their OWN name/phone. Deliberately separate from admin actions
        /// above: no currentAdminUserId parameter, no role/active-status
        /// checks, because this is a person editing themselves, not an
        /// admin acting on someone else. Loads the current row first so
        /// fields this method doesn't touch (RoleID, IsActive, password)
        /// are preserved exactly as they were.</summary>
        public void UpdateProfile(int userId, string fullName, string phoneNumber)
        {
            User u = _dal.SelectById(userId);
            if (u == null) throw new ValidationException("User not found.");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ValidationException("Full name is required.");
            if (fullName.Trim().Length > 100)
                throw new ValidationException("Full name must be 100 characters or fewer.");

            u.FullName = fullName.Trim();
            

            _dal.Update(u); // IsActive passes through unchanged — Update never touches RoleID either
        }

        /// <summary>currentAdminUserId lets the page block an admin from
        /// deactivating their OWN account — without this check, an admin
        /// could lock themselves out with no other admin able to fix it.</summary>
        public void Deactivate(int userId, int currentAdminUserId)
        {
            if (userId <= 0) throw new ValidationException("Invalid user.");
            if (userId == currentAdminUserId)
                throw new ValidationException("You cannot deactivate your own account.");
            if (!_dal.Deactivate(userId)) throw new ValidationException("The user no longer exists.");
        }

        public void Reactivate(int userId)
        {
            if (userId <= 0) throw new ValidationException("Invalid user.");
            if (!_dal.Reactivate(userId)) throw new ValidationException("The user no longer exists.");
        }

        public void ChangeRole(int userId, int roleId, int currentAdminUserId)
        {
            if (userId <= 0) throw new ValidationException("Invalid user.");
            if (roleId <= 0) throw new ValidationException("Invalid role.");
            if (userId == currentAdminUserId)
                throw new ValidationException("You cannot change your own role.");
            if (!_dal.ChangeRole(userId, roleId)) throw new ValidationException("The user no longer exists.");
        }

        /// <summary>Lets an admin manually clear a lockout (see AuthBLL's
        /// MaxFailedAttempts/LockoutDuration) instead of making a locked-out
        /// user wait out the full 15 minutes.</summary>
        public void ClearLockout(int userId)
        {
            if (userId <= 0) throw new ValidationException("Invalid user.");
            _dal.ResetFailedLogins(userId);
        }
    }
}
