using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataAccessLayer.Core.Interfaces.UoW;
using Identity.PermissionManager.BLL.Logic.PermissionManager;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.ViewModels.DTOs;
using Identity.PermissionManager.ViewModels.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.PermissionManager.Api.Controllers.PermissionManagement
{
    [Authorize(Policy = "IdentityUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class PermissionController : BaseApiController
    {
        private readonly PermissionManager<User, Role, int> _permissionManager;

        public PermissionController(IUnitOfWork uow, PermissionManager<User, Role, int> permissionManager) : base(uow)
        {
            this._permissionManager = permissionManager;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var permissionEntities = _uow.Repository<Permission>()
                    .GetRange(null, false, null,
                        p => p.PermissionGroup,
                        p => p.PermissionRoles,
                        per => per.PermissionRoles.Select(pr => pr.Role)
                    );
                var permissionVms = Mapper.Map<IEnumerable<PermissionVm>>(permissionEntities);
                Response<IEnumerable<PermissionVm>> response =
                    new Response<IEnumerable<PermissionVm>>("Get action performed successfully.", permissionVms);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var permissionEntity = _uow.Repository<Permission>()
                    .Get(id, false, p => p.PermissionGroup, p => p.PermissionRoles.Select(pr => pr.Role));
                var permissionVm = Mapper.Map<PermissionVm>(permissionEntity);
                Response<PermissionVm> response =
                    new Response<PermissionVm>(
                        permissionEntity != null ? "Get action performed successfully." : "Permission not exists",
                        permissionVm);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] PermissionDto dto)
        {
            try
            {
                var permissionEntity = Mapper.Map<Permission>(dto);
                permissionEntity = _permissionManager.AddOrUpdatePermission(permissionEntity);
                _uow.Save();
                var result = _uow.Repository<Permission>()
                     .Get(permissionEntity.Id, false, p => p.PermissionGroup, p => p.PermissionRoles.Select(pr => pr.Role));
                var permissionVm = Mapper.Map<PermissionVm>(result);
                Response<PermissionVm> response = new Response<PermissionVm>("Post action performed successfully.",
                    permissionVm);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PermissionDto dto)
        {
            try
            {
                var permissionEntity = Mapper.Map<Permission>(dto);
                permissionEntity.Id = id;
                permissionEntity = _permissionManager.AddOrUpdatePermission(permissionEntity);
                _uow.Save();
                var result = _uow.Repository<Permission>()
                     .Get(permissionEntity.Id, false, p => p.PermissionGroup, p => p.PermissionRoles.Select(pr => pr.Role));
                var permissionVm = Mapper.Map<PermissionVm>(result);
                Response<PermissionVm> response = new Response<PermissionVm>("Put action performed successfully.",
                    permissionVm);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Response<PermissionVm> response = null;
                var permissionEntity = _uow.Repository<Permission>()
                    .Get(id, false, p => p.PermissionGroup, p => p.PermissionRoles.Select(pr => pr.Role));
                if (permissionEntity == null)
                {
                    response = new Response<PermissionVm>("Permission not exists");
                    return Ok(response);

                }
                permissionEntity.PermissionRoles.Clear();
                permissionEntity.PermissionGroupId = null;
                _uow.Repository<Permission>().Update(permissionEntity);
                _uow.Repository<Permission>().Delete(permissionEntity);
                _uow.Save();
                response = new Response<PermissionVm>("Delete action performed successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
