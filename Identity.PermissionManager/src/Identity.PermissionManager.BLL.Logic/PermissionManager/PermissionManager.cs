using System;
using System.Linq;
using DataAccessLayer.Core.Repositories.Interfaces;
using Identity.PermissionManager.BLL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.PermissionManager.BLL.Logic.PermissionManager
{
    public class PermissionManager<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Permission> permissionRepository;
        private readonly IRepository<PermissionRole<Role, int>> permissionRoleRepository;
        private readonly IRepository<PermissionGroup> permissionGroupRepository;
        private readonly IRepository<IdentityUserRole<int>> userRoleRepository;

        public PermissionManager(IRepository<User> userRepository, 
                                 IRepository<Permission> permissionRepository, 
                                 IRepository<PermissionRole<Role, int>> permissionRoleRepository,
                                 IRepository<PermissionGroup> permissionGroupRepository,
                                 IRepository<IdentityUserRole<int>> userRoleRepository)
        {
            this.userRepository = userRepository;
            this.permissionGroupRepository = permissionGroupRepository;
            this.permissionRepository = permissionRepository;
            this.permissionRoleRepository = permissionRoleRepository;
            this.userRoleRepository = userRoleRepository;
        }

        public bool CheckPermissions(PermissionOperator permissionOperator, string[] permissions, string email)
        {
            var userPermissions = userRepository.GetRange(user => user.Email == email, false).Join(userRoleRepository.GetRange(enableTracking: false), u => u.Id, ur => ur.UserId, (u, ur) => new { ur.RoleId })
                                                 .Join(permissionRoleRepository.GetRange(enableTracking: false), r => r.RoleId, per => per.RoleId, (r, per) => new { per.Permission })
                                                 .Distinct()
                                                 .ToList();
            var inter = userPermissions.Where(p => permissions.Any(x => x == p.Permission.Name)).ToList();
            bool result = permissionOperator == PermissionOperator.Or ? inter.Count > 0 : inter.Count() == permissions.Length;
            return result;
        }

        public Permission AddOrUpdatePermission(Permission permissionModel)
        {
            if (string.IsNullOrWhiteSpace(permissionModel.Name) == false)
            {
                Permission permissionEntity = permissionRepository.Get(x => x.Name == permissionModel.Name);
                if (permissionEntity == null)
                {
                    permissionEntity = new Permission()
                    {
                        Name = permissionModel.Name
                    };
                    permissionRepository.Add(permissionEntity);

                }
                else
                {
                    permissionEntity.PermissionGroup = permissionModel.PermissionGroup;
                    permissionEntity.Name = permissionModel.Name;
                    permissionEntity.PermissionGroupId = permissionModel.PermissionGroupId;
                    permissionEntity.Id = permissionModel.Id;
                    permissionEntity.PermissionRoles = permissionModel.PermissionRoles;

                    permissionRepository.Update(permissionEntity);
                }
                return permissionEntity;

                //context.SaveChanges();
                //var result = context.Permissions;
                //result.Include(pr => pr.PermissionRoles).ThenInclude(r => r.Role).Include(g => g.PermissionGroup).Load();
                //return result.FirstOrDefault(x => x.Id == permissionEntity.Id);
            }
            else throw new ArgumentNullException("Permission name cannot be empty.");
        }

        //public PermissionGroup AddOrUpdatePermissionGroup(PermissionGroup groupModel)
        //{
        //    if (string.IsNullOrWhiteSpace(groupModel.GroupName) == false)
        //    {
        //        PermissionGroup groupEntity =
        //            context.PermissionGroups.FirstOrDefault(x => x.GroupName == groupModel.GroupName);
        //        if (groupEntity == null)
        //        {
        //            groupEntity = new PermissionGroup()
        //            {
        //                GroupName = groupModel.GroupName
        //            };
        //            context.PermissionGroups.Add(groupEntity);
        //        }
        //        else
        //        {
        //            groupEntity = new PermissionGroup();
        //            groupEntity.Permissions = groupModel.Permissions;
        //            groupEntity.Id = groupModel.Id;
        //            groupEntity.GroupName = groupModel.GroupName;
        //        }
        //        context.SaveChanges();
        //        var res = context.PermissionGroups.Include(p => p.Permissions);
        //        res.Load();
        //        return res.FirstOrDefault(x => x.Id == groupEntity.Id);
        //    }
        //    else throw new ArgumentNullException("Group name is empty or whitespace.");
        //}

        //public Permission AttachPermissionToGroup(PermissionGroup permissionGroup, Permission permission)
        //{
        //    if (permission != null && permissionGroup != null)
        //    {
        //        permission.PermissionGroup = permissionGroup;
        //        permission.PermissionGroupId = permissionGroup.Id;
        //        context.Permissions.Update(permission);
        //        context.SaveChanges();
        //        var result = context.Permissions;
        //        result.Include(pr => pr.PermissionRoles).ThenInclude(r => r.Role).Include(g => g.PermissionGroup).Load();
        //        return result.FirstOrDefault(x => x.Id == permission.Id);
        //    }
        //    else throw new ArgumentNullException("Permission and Permission Group cannot be null. ");
        //}

        //public Permission DetachPermissionFromGroup(Permission permission)
        //{
        //    permission.PermissionGroup = null;
        //    permission.PermissionGroupId = null;
        //    context.Permissions.Update(permission);
        //    context.SaveChanges();
        //    var result = context.Permissions;
        //    result.Include(pr => pr.PermissionRoles).ThenInclude(r => r.Role).Include(g => g.PermissionGroup).Load();
        //    return result.FirstOrDefault(x => x.Id == permission.Id);
        //}

        //public Permission AttachPermissionToRole(Permission permission, TRole role)
        //{
        //    var permissionRoleEntity = context.PermissionRoles.FirstOrDefault(x => x.PermissionId == permission.Id && x.RoleId.Equals(role.Id));
        //    if (permissionRoleEntity == null)
        //    {
        //        PermissionRole<TRole, TKey> permissionRole = new PermissionRole<TRole, TKey>()
        //        {
        //            PermissionId = permission.Id,
        //            RoleId = role.Id
        //        };
        //        context.PermissionRoles.Add(permissionRole);
        //        context.SaveChanges();
        //        var result = context.Permissions;
        //        result.Include(pr => pr.PermissionRoles).ThenInclude(r => r.Role).Include(g => g.PermissionGroup).Load();
        //        return result.FirstOrDefault(x => x.Id == permission.Id);
        //    }
        //    else
        //        throw new ArgumentException("Given association already exists.");
        //}

        //public Permission DetachPermissionFromRole(Permission permission, TRole role)
        //{
        //    // potentially problems with ref types, check if string is working
        //    var permissionRole = context.PermissionRoles
        //                                .Include(p => p.Permission).ThenInclude(g => g.PermissionGroup)
        //                                .Include(r => r.Role)
        //                                .FirstOrDefault(x => x.PermissionId == permission.Id && x.RoleId.Equals(role.Id));
        //    if (permissionRole != null)
        //    {
        //        context.PermissionRoles.Remove(permissionRole);
        //        context.SaveChanges();
        //        var result = context.Permissions;
        //        result.Include(pr => pr.PermissionRoles).ThenInclude(r => r.Role).Include(g => g.PermissionGroup).Load();
        //        return result.FirstOrDefault(x => x.Id == permission.Id);

        //    }
        //    else
        //        throw new ArgumentException("There is no such association in the database.");
        //}
    }
}
