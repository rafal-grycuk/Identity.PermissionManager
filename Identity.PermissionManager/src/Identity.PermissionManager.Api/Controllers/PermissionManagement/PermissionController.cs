//using System;
//using System.Collections.Generic;
//using AutoMapper;
//using DataAccessLayer.Core.UoW;
//using Identity.PermissionManager.BLL.Logic.PermissionManager;
//using Identity.PermissionManager.BLL.Models;
//using Identity.PermissionManager.ViewModels.DTOs;
//using Identity.PermissionManager.ViewModels.VMs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

//namespace Identity.PermissionManager.Api.Controllers.PermissionManagement
//{
//    [Authorize(Policy = "IdentityUser", Roles = "Admin")]
//    [Route("api/[controller]")]
//    public class PermissionController : BaseApiController
//    {
//        private readonly PermissionManager<User, Role, int> permissionManager;

//        public PermissionController(IUnitOfWork uow, PermissionManager<User, Role, int> permissionManager) : base(uow)
//        {
//            this.permissionManager = permissionManager;
//        }

//        // GET: api/values
//        [HttpGet]
//        public IActionResult Get()
//        {
//            try
//            {
//                var permissionEntities = uow.Context.Permissions
//                    .Include(p => p.PermissionGroup)
//                    .Include(p => p.PermissionRoles)
//                    .ThenInclude(pr => pr.Role);
//                var permissionVms = Mapper.Map<IEnumerable<PermissionVm>>(permissionEntities);
//                Response<IEnumerable<PermissionVm>> response =
//                    new Response<IEnumerable<PermissionVm>>("Get action performed successfully.", permissionVms);
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // GET api/values/5
//        [HttpGet("{id}")]
//        public IActionResult Get(int id)
//        {
//            try
//            {
//                var permissionEntity = uow.Context.Permissions
//                    .Include(p => p.PermissionGroup)
//                    .Include(p => p.PermissionRoles)
//                    .ThenInclude(pr => pr.Role)
//                    .FirstOrDefault(x => x.Id == id);
//                var permissionVm = Mapper.Map<PermissionVm>(permissionEntity);
//                Response<PermissionVm> response =
//                    new Response<PermissionVm>(
//                        permissionEntity != null ? "Get action performed successfully." : "Permission not exists",
//                        permissionVm);
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // POST api/values
//        [HttpPost]
//        public IActionResult Post([FromBody] PermissionDto dto)
//        {
//            try
//            {
//                var permissionEntity = Mapper.Map<Permission>(dto);
//                permissionEntity = permissionManager.AddOrUpdatePermission(permissionEntity);
//                uow.Save();
//                var result = uow.Repository<Permission>()
//                    .Get(permissionEntity.Id, false, per => per.PermissionGroup, per => per.PermissionRoles);
//                result.Include(pr => pr.PermissionRoles).ThenInclude(r => r.Role).Include(g => g.PermissionGroup).Load();
//                //return result.FirstOrDefault(x => x.Id == permissionEntity.Id);
//                var permissionVm = Mapper.Map<PermissionVm>(permissionEntity);
//                Response<PermissionVm> response = new Response<PermissionVm>("Post action performed successfully.",
//                    permissionVm);
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // PUT api/values/5
//        [HttpPut("{id}")]
//        public IActionResult Put(int id, [FromBody] PermissionDto dto)
//        {
//            try
//            {
//                var permissionEntity = Mapper.Map<Permission>(dto);
//                permissionEntity.Id = id;
//                permissionEntity = permissionManager.AddOrUpdatePermission(permissionEntity);
//                var permissionVm = Mapper.Map<PermissionVm>(permissionEntity);
//                Response<PermissionVm> response = new Response<PermissionVm>("Put action performed successfully.",
//                    permissionVm);
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // DELETE api/values/5
//        [HttpDelete("{id}")]
//        public IActionResult Delete(int id)
//        {
//            try
//            {
//                Response<PermissionVm> response = null;
//                var permissionEntity = uow.Context.Permissions
//                    .Include(p => p.PermissionGroup)
//                    .Include(p => p.PermissionRoles)
//                    .ThenInclude(pr => pr.Role)
//                    .FirstOrDefault(x => x.Id == id);
//                if (permissionEntity == null)
//                {
//                    response = new Response<PermissionVm>("Permission not exists");
//                    return Ok(response);

//                }
//                permissionEntity.PermissionRoles.Clear();
//                permissionEntity.PermissionGroupId = null;
//                uow.PermissionRepo.Update(permissionEntity);
//                uow.PermissionRepo.Delete(permissionEntity);
//                uow.SaveChanges();
//                response = new Response<PermissionVm>("Delete action performed successfully.");
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }


    