using System;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLayer.Core.Interfaces.UoW;
using Identity.PermissionManager.BLL.Logic.PermissionManager;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.ViewModels.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.PermissionManager.Api.Controllers.PermissionManagement
{
    [Route("api/[controller]")]
    [Authorize(Policy = "IdentityUser", Roles = "Admin")]
    public class AdministrationController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly PermissionManager<User, Role, int> _permissionManager;


        // GET: api/values
        public AdministrationController(IUnitOfWork uow, UserManager<User> userManager, RoleManager<Role> roleManager,
            PermissionManager<User, Role, int> permissionManager) : base(uow)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._permissionManager = permissionManager;
        }

        [HttpPost("AddPermissionGroup")]
        public IActionResult AddPermissionGroup(string permissionGroupName)
        {
            try
            {
                var permissionGroup = new PermissionGroup()
                {
                    GroupName = permissionGroupName
                };
                var res = _permissionManager.AddOrUpdatePermissionGroup(permissionGroup);
                var permissionGroupVm = Mapper.Map<PermissionGroupVm>(res);
                return Ok(permissionGroupVm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("AttachPermissionToRole")]
        public IActionResult AttachPermissionToRole(string roleName, string permissionName)
        {
            try
            {
                var role = _uow.Repository<Role>().Get(r => r.Name == roleName);
                var permission = _uow.Repository<Permission>().Get(per => per.Name == permissionName);
                if (role != null && permission != null)
                {
                    var resp = _permissionManager.AttachPermissionToRole(permission, role);
                    var respVm = Mapper.Map<PermissionVm>(resp);
                    return Ok(respVm);
                }
                else throw new ArgumentException("Invalid Arguments");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DetachPermissionFromRole")]
        public IActionResult DetachPermissionFromRole(string roleName, string permissionName)
        {
            try
            {
                var role = _uow.Repository<Role>().Get(r => r.Name == roleName);
                var permission = _uow.Repository<Permission>().Get(per => per.Name == permissionName);
                if (role != null && permission != null)
                {
                    var resp = _permissionManager.DetachPermissionFromRole(permission, role);
                    var respVm = Mapper.Map<PermissionVm>(resp);
                    return Ok(respVm);
                }
                else throw new ArgumentException("Invalid Arguments");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("AttachPermissionToGroup")]
        public IActionResult AttachPermissionToGroup(string permissionName, string groupName)
        {
            try
            {
                var group = _uow.Repository<PermissionGroup>().Get(g => g.GroupName == groupName);
                var permission = _uow.Repository<Permission>().Get(per => per.Name == permissionName);
                if (group != null && permission != null)
                {
                    var resp = _permissionManager.AttachPermissionToGroup(group, permission);
                    var respVm = Mapper.Map<PermissionVm>(resp);
                    return Ok(respVm);
                }
                else throw new ArgumentException("Invalid Arguments");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DetachPermissionFromGroup")]
        public IActionResult DetachPermissionFromGroup(string permissionName)
        {
            try
            {
                var permission = _uow.Repository<Permission>().Get(per => per.Name == permissionName);
                if (permission != null)
                {
                    var resp = _permissionManager.DetachPermissionFromGroup(permission);
                    var respVm = Mapper.Map<PermissionVm>(resp);
                    return Ok(respVm);
                }
                else throw new ArgumentException("Invalid Arguments");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AttachUserToRole")]
        public async Task<IActionResult> AttachUserToRole(string roleName, string userName)
        {
            try
            {
                var role = _uow.Repository<Role>().Get(r => r.Name == roleName);
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null && role != null)
                {
                    var response = await _userManager.AddToRoleAsync(user, role.Name);
                    if (response.Succeeded)
                    {
                        return Ok("Attach successfull");
                    }
                    else return StatusCode(500, response.Errors);

                }
                else
                    return StatusCode(500, "Given user or role does not exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DetachUserFromRole")]
        public async Task<IActionResult> DetachUserFromRole(string roleName, string userName)
        {
            try
            {
                var role = _uow.Repository<Role>().Get(r => r.Name == roleName);
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null && role != null)
                {
                    var response = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    if (response.Succeeded)
                    {
                        return Ok("Detach successfull");
                    }
                    else return StatusCode(500, response.Errors);
                }
                else
                    return StatusCode(500, "Given user or role does not exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
