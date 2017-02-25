using System;
using Identity.PermissionManager.BLL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.PermissionManager.DAL.EF
{
    public class PermissionManagerDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly ConnectionStringDto _connectionStringDto;
        public PermissionManagerDbContext(ConnectionStringDto connectionStringDto)
        {
            this._connectionStringDto = connectionStringDto;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           // optionsBuilder.UseSqlServer(this._connectionStringDto.ConnectionString);
            optionsBuilder.UseInMemoryDatabase();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PermissionManager.BLL.Models.Permission>()
            .ToTable("Permission");

            builder.Entity<PermissionGroup>()
            .ToTable("PermissionGroup");


            builder.Entity<PermissionRole<TRole, TKey>>()
                .ToTable("PermissionRole")
                .HasKey(t => new {t.PermissionId, t.RoleId});


        }

        public DbSet<PermissionManager.BLL.Models.Permission> Permissions { get; set; }
        public DbSet<PermissionRole<TRole, TKey>> PermissionRoles { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }

    }
}