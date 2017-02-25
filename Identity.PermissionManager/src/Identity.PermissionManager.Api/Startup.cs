using System;
using System.Text;
using DataAccessLayer.Core.EntityFramework.Repositories;
using DataAccessLayer.Core.EntityFramework.UoW;
using DataAccessLayer.Core.Interfaces.Repositories;
using DataAccessLayer.Core.Interfaces.UoW;
using Identity.PermissionManager.Api.Configuration;
using Identity.PermissionManager.BLL.Models;
using Identity.PermissionManager.DAL.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Identity.PermissionManager.BLL.Logic.PermissionManager;
using Identity.PermissionManager.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.PermissionManager.Api
{
    public class Startup
    {
        private readonly string _connectionString;
        private const string SecretKey = "M6Lc*rmr5s7?_YE__pnq9$vR87T*VA6Dt6*z&Cz4";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            if (env.IsEnvironment("Development"))
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            if (env.IsDevelopment())
            {
                _connectionString = Configuration.GetConnectionString("DefaultConnection");
                    //Configuration["DefaultConnection"];
            }
            else if (env.IsProduction())
            {
                _connectionString = Configuration.GetConnectionString("DefaultConnection");
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Framework Services
            // Add framework services.
            services.AddOptions();

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<PermissionManagerDbContext<User, Role, int>>(options =>
               //options.UseSqlServer(_connectionString) // existing database
              options.UseInMemoryDatabase() // in memory DB
            );

            services.AddIdentity<User, Role>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
                o.Cookies.ApplicationCookie.AutomaticChallenge = false;
                o.Cookies.ApplicationCookie.LoginPath = null;
            })
                .AddEntityFrameworkStores<PermissionManagerDbContext<User, Role, int>, int>()
                .AddDefaultTokenProviders();

            //services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", corsBuilder.Build());
            });


            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IdentityUser",
                                  policy => policy.RequireClaim("IdentityUser", "User"));
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));


            // Configure JwtIssuerOptions
            int tokenExpirationMinutes = 20;

#if DEBUG
            tokenExpirationMinutes = 150000;

#endif


            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
                options.ValidFor = TimeSpan.FromMinutes(tokenExpirationMinutes);


            });

            #endregion

            #region Our Services

            var cs = new ConnectionStringDto() { ConnectionString = _connectionString };
            services.AddSingleton<ConnectionStringDto>(cs);
            services.AddScoped<DbContext, PermissionManagerDbContext<User, Role, int>>();
            services.AddScoped<DbContextOptions<PermissionManagerDbContext<User, Role, int>>>();
            services.AddScoped<PermissionManager<User, Role, int>>();
            services.AddScoped<PermissionManagerDbContext<User, Role, int>>();
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped<TokenProviderMiddleware>();

            var mappingConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddAdminMapping();
            });

            services.AddSingleton(x => mappingConfig.CreateMapper());
            services.AddScoped<IRepository<User>, EntityFrameworkRepository<User>>();
           // services.AddScoped<IRepository<IdentityUser<int>>, EntityFrameworkRepository<IdentityUser<int>>>();
            services.AddScoped<IRepository<Role>, EntityFrameworkRepository<Role>>();
            services.AddScoped<IRepository<IdentityUserRole<int>>, EntityFrameworkRepository<IdentityUserRole<int>>>();
            services.AddScoped<IRepository<Permission>, EntityFrameworkRepository<Permission>>();
            services.AddScoped<IRepository<PermissionGroup>, EntityFrameworkRepository<PermissionGroup>>();
            services.AddScoped<IRepository<PermissionRole<Role, int>>, EntityFrameworkRepository<PermissionRole<Role, int>>>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            app.UseIdentity();
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,


            });

            //http://www.fiyazhasan.me/angularjs-anti-forgery-with-asp-net-core/
            //app.Use(next => context =>
            //{
            //    if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
            //        string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
            //    {
            //        var tokens = antiforgery.GetAndStoreTokens(context);
            //        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
            //            new CookieOptions() { HttpOnly = false });
            //    }

            //    return next(context);
            //});

            app.UseCors("AllowAll");

           // app.UseApplicationInsightsRequestTelemetry();

          //  app.UseApplicationInsightsExceptionTelemetry();
            var uow = app.ApplicationServices.GetService<IUnitOfWork>();
            var userManager = app.ApplicationServices.GetService<UserManager<User>>();
            var roleManager = app.ApplicationServices.GetService<RoleManager<Role>>();
            Seed.SeedData(uow, roleManager, userManager);
            app.UseMvc();
        }
    }
}
