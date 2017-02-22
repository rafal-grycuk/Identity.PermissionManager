using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Core.UoW;
using Identity.PermissionManager.BLL.Models;

namespace Identity.PermissionManager.DAL.EF
{
    public static class Seed
    {
        public static void AddPermission(IUnitOfWork uow)
        {
            Permission permission = new Permission()
            {
                Name = "Read"
            };
            uow.Repository<Permission>().Add(permission);
            uow.Save();
        }
    }
}
