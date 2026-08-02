using System.Collections.Generic;
using System.Data.SqlClient;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Helpers;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    public class CategoryBLL
    {
        private readonly CategoryDAL _dal = new CategoryDAL();

        public List<Category> GetAll() { return _dal.SelectAll(); }
        public List<Category> GetActive() { return _dal.SelectActive(); }
        public Category GetById(int categoryId) { return categoryId <= 0 ? null : _dal.SelectById(categoryId); }

        public int Add(Category c)
        {
            Validate(c);
            try
            {
                return _dal.Insert(c);
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, uniqueMessage: "A category with this name already exists.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "CategoryBLL.Add");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        public void Edit(Category c)
        {
            if (c == null || c.CategoryID <= 0) throw new ValidationException("Invalid category.");
            Validate(c);

            try
            {
                if (!_dal.Update(c))
                    throw new ValidationException("The category no longer exists. It may have been deleted.");
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, uniqueMessage: "A category with this name already exists.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "CategoryBLL.Edit");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        /// <summary>
        /// Deleting a category that still has courses hits Courses' FK
        /// (error 547) — SqlErrorHelper turns that into a message that tells
        /// the admin WHY, instead of a raw database error.
        /// </summary>
        public void Remove(int categoryId)
        {
            if (categoryId <= 0) throw new ValidationException("Invalid category.");

            try
            {
                if (!_dal.Delete(categoryId))
                    throw new ValidationException("The category no longer exists.");
            }
            catch (SqlException sqlEx)
            {
                ValidationException vex = SqlErrorHelper.Translate(
                    sqlEx, fkMessage: "This category still has courses assigned to it. Move or delete those courses first.");
                if (vex != null) throw vex;

                ErrorLogger.Log(sqlEx, "CategoryBLL.Remove");
                throw new ValidationException("A database error occurred. Please try again.");
            }
        }

        private void Validate(Category c)
        {
            if (c == null) throw new ValidationException("No category data was supplied.");
            if (string.IsNullOrWhiteSpace(c.CategoryName)) throw new ValidationException("Category name is required.");
            c.CategoryName = c.CategoryName.Trim();
            if (c.CategoryName.Length > 60) throw new ValidationException("Category name must be 60 characters or fewer.");
            if (!string.IsNullOrEmpty(c.Description) && c.Description.Length > 250)
                throw new ValidationException("Description must be 250 characters or fewer.");
        }
    }
}
