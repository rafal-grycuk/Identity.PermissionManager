using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLayer.Core.Interfaces.UoW;
using Identity.PermissionManager.BLL.Logic.Filters;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.Middleware;
using Identity.PermissionManager.Utilities.Extensions;
using Identity.PermissionManager.ViewModels.DTOs;
using Identity.PermissionManager.ViewModels.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.PermissionManager.Api.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;

        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly TokenProviderMiddleware _tokenProviderMiddleware;

        public AccountController(IUnitOfWork uow,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<JwtIssuerOptions> jwtOptions,
            ILoggerFactory loggerFactory,
            TokenProviderMiddleware tokenProviderMiddleware
        )
            : base(uow)
        {
            this._userManager = userManager;
            this._jwtOptions = jwtOptions.Value;
            this._tokenProviderMiddleware = tokenProviderMiddleware;
            tokenProviderMiddleware.ThrowIfInvalidOptions(this._jwtOptions);

            _logger = loggerFactory.CreateLogger<AccountController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        // GET: api/values
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = Mapper.Map<User>(dto);
                    var result = await _userManager.CreateAsync(entity, dto.Password);
                    if (result.Succeeded)
                        return Ok(new {success = true});
                    else
                        return StatusCode(500, result.Errors.GetErrorsString());
                }
                else
                {
                    return StatusCode(500, "Invalid data model.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.Message + " Inner Message " + ex?.InnerException?.Message);
            }
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginUserVm applicationUser)
        {
            var identity = await _tokenProviderMiddleware.GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation(
                    $"Invalid username ({applicationUser.Email}) or password ({applicationUser.Password})");
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    _tokenProviderMiddleware.ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("IdentityUser")
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int) _jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        [HttpGet]
        [HasPermission(PermissionOperator.And, "Read", "Execute", "Write")]
        public IActionResult Get()
        {
            var authenticatedUser = ((ClaimsIdentity) User?.Identity)?.Claims?.First()?.Value;
            if (authenticatedUser != null)
            {
                var user = _uow.Repository<User>().Get(u => u.Email == authenticatedUser, false);
                if (user != null)
                {

                    var userIndividualVm = Mapper.Map<BaseUserInfoVm>(user);
                    return Ok(userIndividualVm);

                }
                else
                    return StatusCode(500, "Given user does not exist in the database.");

            }
            else return Unauthorized();
        }
    }
}
