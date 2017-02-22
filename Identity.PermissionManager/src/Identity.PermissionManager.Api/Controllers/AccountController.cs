using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLayer.Core.UoW;
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
        private readonly UserManager<User> userManager;

        private readonly JwtIssuerOptions jwtOptions;
        private readonly ILogger logger;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly TokenProviderMiddleware tokenProviderMiddleware;

        public AccountController(IUnitOfWork uow,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<JwtIssuerOptions> jwtOptions,
            ILoggerFactory loggerFactory,
            TokenProviderMiddleware tokenProviderMiddleware
        )
            : base(uow)
        {
            this.userManager = userManager;
            this.jwtOptions = jwtOptions.Value;
            this.tokenProviderMiddleware = tokenProviderMiddleware;
            tokenProviderMiddleware.ThrowIfInvalidOptions(this.jwtOptions);

            logger = loggerFactory.CreateLogger<AccountController>();

            serializerSettings = new JsonSerializerSettings
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
                    var result = await userManager.CreateAsync(entity, dto.Password);
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
            var identity = await tokenProviderMiddleware.GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                logger.LogInformation(
                    $"Invalid username ({applicationUser.Email}) or password ({applicationUser.Password})");
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    tokenProviderMiddleware.ToUnixEpochDate(jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("IdentityUser")
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: jwtOptions.NotBefore,
                expires: jwtOptions.Expiration,
                signingCredentials: jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int) jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, serializerSettings);
            return new OkObjectResult(json);
        }

        [HttpGet]
        [HasPermission(PermissionOperator.Or, "Read", "Execute", "Write")]
        public IActionResult Get()
        {
            var authenticatedUser = ((ClaimsIdentity) User?.Identity)?.Claims?.First()?.Value;
            if (authenticatedUser != null)
            {
                var user = uow.Repository<User>().Get(u => u.Email == authenticatedUser, false);
                if (user != null)
                {

                    var userIndividualVM = Mapper.Map<BaseUserInfoVm>(user);
                    return Ok(userIndividualVM);

                }
                else
                    return StatusCode(500, "Given user does not exist in the database.");

            }
            else return Unauthorized();
        }
    }
}
