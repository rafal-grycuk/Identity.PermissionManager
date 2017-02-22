using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.ViewModels.DTOs;
using Identity.PermissionManager.ViewModels.VMs;

namespace BeThere.WebAPI.Configuration
{
    public static class AutoMapperConfig
    {
        public static IMapperConfigurationExpression AddAdminMapping(this IMapperConfigurationExpression configurationExpression)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, RegisterUserDto>()
                    .ForMember(x => x.Password, (IMemberConfigurationExpression<User, RegisterUserDto, string> y) => y.Ignore())
                    .ForMember(x => x.ConfirmPassword, (IMemberConfigurationExpression<User, RegisterUserDto, string> y) => y.Ignore())
                    .ReverseMap();

                cfg.CreateMap<Permission, PermissionVm>()
                    .ForMember(x => x.PermissionGroup, (IMemberConfigurationExpression<Permission, PermissionVm, string> y) => y.MapFrom(z => z.PermissionGroup.GroupName))
                    .ForMember(x => x.Roles, (IMemberConfigurationExpression<Permission, PermissionVm, ICollection<string>> y) => y.MapFrom(z => z.PermissionRoles.Select((PermissionRole<Role, int> pr) => pr.Role.Name)));

                cfg.CreateMap<PermissionDto, Permission>();

                cfg.CreateMap<PermissionGroup, PermissionGroupVm>();

                cfg.CreateMap<Role, RoleVm>()
                    .ForMember(x => x.Permissions, (IMemberConfigurationExpression<Role, RoleVm, ICollection<string>> z) => z.MapFrom(y => y.PermissionRoles.Select((PermissionRole<Role, int> p) => p.Permission.Name)));

                cfg.CreateMap<RoleDto, Role>();

                cfg.CreateMap<User, UserVm>();
            });


            return configurationExpression;
        }



    }
}
