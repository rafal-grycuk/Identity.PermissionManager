using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLayer.Core.Interfaces.UoW;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.Utilities.Extensions;
using Identity.PermissionManager.ViewModels.DTOs;
using Identity.PermissionManager.ViewModels.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.PermissionManager.Api.Controllers.PermissionManagement
{
    [Authorize(Policy = "IdentityUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        private readonly UserManager<User> _userManager;

        public UserController(IUnitOfWork uow, UserManager<User> userManager) : base(uow)
        {
            this._userManager = userManager;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var usersVm = new List<UserVm>();
                var users = _uow.Repository<User>().GetRange(null, false, null, r => r.Roles);
                foreach (var user in users)
                {
                    var roles = _uow.Repository<Role>()
                    .GetRange(r => r.Users.Any(x => x.UserId == user.Id), false, null ,pr => pr.PermissionRoles.Select(p => p.Permission));
                    var userVm = Mapper.Map<UserVm>(user);
                    userVm.RolesList = Mapper.Map<IEnumerable<RoleVm>>(roles);
                    usersVm.Add(userVm);
                }
                Response<IEnumerable<UserVm>> response = new Response<IEnumerable<UserVm>>("Get action performed successfully.", usersVm);
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
                var user = _uow.Repository<User>().Get(id, false, r => r.Roles);
                 
                var roles = _uow.Repository<Role>()
                    .GetRange(r => r.Users.Any(x => x.UserId == user.Id), false, null, pr => pr.PermissionRoles.Select(p => p.Permission));
                var userVm = Mapper.Map<UserVm>(user);
                userVm.RolesList = Mapper.Map<IEnumerable<RoleVm>>(roles);

                Response<UserVm> response = new Response<UserVm>("Get action performed successfully.", userVm);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto dto)
        {
            try
            {
var userEntity = Mapper.Map<User>(dto);
                IdentityResult result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded == true)
                {
                    var userVm = Mapper.Map<UserVm>(userEntity);
                    Response<UserVm> response = new Response<UserVm>("Post action performed successfully.", userVm);
                    return Ok(response);
                }
                else
                    return StatusCode(500, result.Errors.GetErrorsString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto dto)
        {
            try
            {
                var userEntity = await _userManager.FindByIdAsync(id.ToString());
                userEntity.Email = dto.Email ?? userEntity.Email;
                userEntity.FirstName = dto.FirstName ?? userEntity.FirstName;
                userEntity.LastName = dto.LastName ?? userEntity.LastName;
                userEntity.PhoneNumber = dto.PhoneNumber ?? userEntity.PhoneNumber;
                userEntity.UserName = dto.UserName?? userEntity.UserName ;
                IdentityResult result = await _userManager.UpdateAsync(userEntity);
                if (result.Succeeded == true)
                {
                    var userVm = Mapper.Map<UserVm>(userEntity);
                    Response<UserVm> response = new Response<UserVm>("Put action performed successfully.", userVm);
                    return Ok(response);
                }
                else
                    return StatusCode(500, result.Errors.GetErrorsString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userEntity = await _userManager.FindByIdAsync(id.ToString());
                Response<PermissionVm> response = null;
                if (userEntity == null)
                {
                    response = new Response<PermissionVm>("User not exists");
                    return Ok(response);
                }
                var result = await _userManager.DeleteAsync(userEntity);
                if (result.Succeeded == true)
                {
                    response = new Response<PermissionVm>("Delete action performed successfully.");
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, result.Errors.GetErrorsString());
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
