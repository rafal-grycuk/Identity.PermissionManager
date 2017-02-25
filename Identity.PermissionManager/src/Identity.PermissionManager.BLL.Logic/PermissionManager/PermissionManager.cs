using System;
using System.Linq;
using DataAccessLayer.Core.Interfaces.Repositories;
using Identity.PermissionManager.BLL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.PermissionManager.BLL.Logic.PermissionManager
{
    public class PermissionManager<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IRepository<TUser> _userRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<PermissionRole<TRole, TKey>> _permissionRoleRepository;
        private readonly IRepository<PermissionGroup> _permissionGroupRepository;
        private readonly IRepository<IdentityUserRole<TKey>> _userRoleRepository;

        public PermissionManager(IRepository<TUser> userRepository,
                                 IRepository<Permission> permissionRepository,
                                 IRepository<PermissionRole<TRole, TKey>> permissionRoleRepository,
                                 IRepository<PermissionGroup> permissionGroupRepository,
                                 IRepository<IdentityUserRole<TKey>> userRoleRepository)
        {
            this._userRepository = userRepository;
            this._permissionGroupRepository = permissionGroupRepository;
            this._permissionRepository = permissionRepository;
            this._permissionRoleRepository = permissionRoleRepository;
            this._userRoleRepository = userRoleRepository;
        }

        public bool CheckPermissions(PermissionOperator permissionOperator, string[] permissions, string email)
        {
            var userPermissions = _userRepository.GetRange(user => user.Email == email, false).Join(_userRoleRepository.GetRange(enableTracking: false), u => u.Id, ur => ur.UserId, (u, ur) => new { ur.RoleId })
                                                 .Join(_permissionRoleRepository.GetRange(enableTracking: false), r => r.RoleId, per => per.RoleId, (r, per) => new { per.Permission })
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
                Permission permissionEntity = _permissionRepository.Get(permissionModel.Id);
                if (permissionEntity == null)
                {
                    permissionEntity = _permissionRepository.Add(permissionModel);
                }
                else
                {
                    permissionEntity.PermissionGroup = permissionModel.PermissionGroup;
                    permissionEntity.Name = permissionModel.Name;
                    permissionEntity.PermissionGroupId = permissionModel.PermissionGroupId;
                    permissionEntity.Id = permissionModel.Id;
                    permissionEntity.PermissionRoles = permissionModel.PermissionRoles;
                    _permissionRepository.Update(permissionEntity);
                }
                return permissionEntity;
            }
            else throw new ArgumentNullException("Permission name cannot be empty.");
        }

        public PermissionGroup AddOrUpdatePermissionGroup(PermissionGroup groupModel)
        {
            if (string.IsNullOrWhiteSpace(groupModel.GroupName) == false)
            {
                PermissionGroup groupEntity = _permissionGroupRepository.Get(x => x.GroupName == groupModel.GroupName);
                if (groupEntity == null)
                {
                    groupEntity = new PermissionGroup()
                    {
                        GroupName = groupModel.GroupName
                    };
                    _permissionGroupRepository.Add(groupEntity);
                }
                else
                {
                    groupEntity = new PermissionGroup
                    {
                        Permissions = groupModel.Permissions,
                        Id = groupModel.Id,
                        GroupName = groupModel.GroupName
                    };
                }

                return groupEntity;
            }
            else throw new ArgumentNullException("Group name is empty or whitespace.");
        }

        public Permission AttachPermissionToGroup(PermissionGroup permissionGroup, Permission permission)
        {
            if (permission != null && permissionGroup != null)
            {
                permission.PermissionGroup = permissionGroup;
                permission.PermissionGroupId = permissionGroup.Id;
                permission = _permissionRepository.Update(permission);
                return permission;
            }
            else throw new ArgumentNullException("Permission and Permission Group cannot be null. ");
        }

        public Permission DetachPermissionFromGroup(Permission permission)
        {
            permission.PermissionGroup = null;
            permission.PermissionGroupId = null;
            permission = _permissionRepository.Update(permission);
            return permission;
        }

        public Permission AttachPermissionToRole(Permission permission, TRole role)
        {
            var permissionRoleEntity = _permissionRoleRepository.Get(x => x.PermissionId == permission.Id && x.RoleId.Equals(role.Id));
            if (permissionRoleEntity == null)
            {
                PermissionRole<TRole, TKey> permissionRole = new PermissionRole<TRole, TKey>()
                {
                    PermissionId = permission.Id,
                    RoleId = role.Id
                };
                _permissionRoleRepository.Add(permissionRole);

                return permission;
            }
            else
                throw new ArgumentException("Given association already exists.");
        }

        public Permission DetachPermissionFromRole(Permission permission, TRole role)
        {
            // potentially problems with ref types, check if string is working
            var permissionRole =
                _permissionRoleRepository.Get(x => x.PermissionId == permission.Id && x.RoleId.Equals(role.Id), true);
            if (permissionRole != null)
            {
                _permissionRoleRepository.Delete(permissionRole);

                return permission;
            }

            else
                throw new ArgumentException("There is no such association in the database.");
        }
    }
}
