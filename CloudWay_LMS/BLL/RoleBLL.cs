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
    public class RoleBLL : PersistentConnection
    {
       
        
            private readonly RoleDAL _dal = new RoleDAL();
            public List<Role> GetAll() { return _dal.SelectAll(); }
        }
    
}