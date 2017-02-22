//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using DataAccessLayer.Core.UoW;
//using Identity.PermissionManager.Api.Controllers;
//using Identity.PermissionManager.BLL.Models;
//using Identity.PermissionManager.Utilities.Extensions;
//using Identity.PermissionManager.ViewModels.DTOs;
//using Identity.PermissionManager.ViewModels.VMs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

//namespace BeThere.WebAPI.Controllers.PermissionManegment
//{
//    [Authorize(Policy = "IdentityUser", Roles = "Admin")]
//    [Route("api/[controller]")]
//    public class RoleController : BaseApiController
//    {
//        private readonly RoleManager<Role> roleManager;
//        public RoleController(IUnitOfWork uow, RoleManager<Role> roleManager) : base(uow)
//        {
//            this.roleManager = roleManager;
//        }

//        // GET: api/values
//        [HttpGet]
//        public IActionResult Get()
//        {
//            try
//            {
//                var roles = uow.Context.Roles
//                    .Include(pr => pr.PermissionRoles)
//                    .ThenInclude(p => p.Permission);
//                var rolesVm = Mapper.Map<IEnumerable<RoleVm>>(roles);
//                Response<IEnumerable<RoleVm>> response = new Response<IEnumerable<RoleVm>>("Get action performed successfully.", rolesVm);
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
//                var role = uow.Context.Roles
//                    .Include(pr => pr.PermissionRoles)
//                    .ThenInclude(p => p.Permission)
//                    .FirstOrDefault(r => r.Id == id);
//                var roleVm = Mapper.Map<RoleVm>(role);
//                Response<RoleVm> response = new Response<RoleVm>(role != null ? "Get action performed successfully." : "Role not exist.", roleVm);
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // POST api/values
//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody]RoleDto dto)
//        {
//            try
//            {
//                var roleEntity = Mapper.Map<Role>(dto);
//                IdentityResult result = await roleManager.CreateAsync(roleEntity);
//                if (result.Succeeded == true)
//                {
//                    var roleVm = Mapper.Map<RoleVm>(roleEntity);
//                    Response<RoleVm> response = new Response<RoleVm>("Post action performed successfully.", roleVm);
//                    return Ok(response);
//                }
//                else
//                    return StatusCode(500, result.Errors.GetErrorsString());
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // PUT api/values/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, [FromBody]RoleDto dto)
//        {
//            try
//            {
//                var roleEntity = await roleManager.FindByIdAsync(id.ToString());
//                roleEntity.Name = dto.Name;
//                IdentityResult result = await roleManager.UpdateAsync(roleEntity);
//                if (result.Succeeded == true)
//                {
//                    var roleVm = Mapper.Map<RoleVm>(roleEntity);
//                    Response<RoleVm> response = new Response<RoleVm>("Put action performed successfully.", roleVm);
//                    return Ok(response);
//                }
//                else
//                    return StatusCode(500, result.Errors.GetErrorsString());
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // DELETE api/values/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            try
//            {
//                var role = uow.Context.Roles
//                       .Include(pr => pr.PermissionRoles)
//                       .ThenInclude(p => p.Permission)
//                       .FirstOrDefault(r => r.Id == id);
//                Response<PermissionVm> response = null;
//                if (role == null)
//                {
//                    response = new Response<PermissionVm>("Role not exists");
//                    return Ok(response);
//                }
//                var result = await roleManager.DeleteAsync(role);
//                if (result.Succeeded == true)
//                {
//                    response = new Response<PermissionVm>("Delete action performed successfully.");
//                    return Ok(response);
//                }
//                else
//                {
//                    return StatusCode(500, result.Errors.GetErrorsString());
//                }

//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}
