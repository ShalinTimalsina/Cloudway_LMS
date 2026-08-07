using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Techspire_LMS.Data_Access_Layer;
using Techspire_LMS.Models;

namespace Techspire_LMS.BLL
{
    public class RoleBLL : PersistentConnection
    {
       
        
            private readonly RoleDAL _dal = new RoleDAL();
            public List<Role> GetAll() { return _dal.SelectAll(); }
        }
    
}