using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DataAccessLayer.Core.UoW;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.ViewModels.VMs;
using Microsoft.AspNetCore.Identity;

namespace Identity.PermissionManager.Middleware
{
    public class TokenProviderMiddleware
    {
        private readonly IUnitOfWork uow;
        private readonly SignInManager<User> signInManager;
        public TokenProviderMiddleware(IUnitOfWork uow, SignInManager<User> signInManager)
        {
            this.uow = uow;
            this.signInManager = signInManager;
        }
        public void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        public long ToUnixEpochDate(DateTime date)
         => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);


        public Task<ClaimsIdentity> GetClaimsIdentity(LoginUserVm dto)
        {
            var user = uow.Repository<User>().Get(u => u.Email == dto.Email, false);
            if (user != null)
            {
                SignInResult result = null;

                result = signInManager.PasswordSignInAsync(user.UserName, dto.Password, true, false).Result;


                if (result.Succeeded)
                {
                    return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
                        new[]
                        {
                            new Claim("IdentityUser", "User")
                        }));
                }
            }
            return Task.FromResult<ClaimsIdentity>(null);
            }

        }
    }