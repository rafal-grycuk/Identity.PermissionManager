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
        private readonly ConnectionStringDto _connectionStringDto;
        private readonly DbLocation _databaseLocation;
        public PermissionManagerDbContext(ConnectionStringDto connectionStringDto, DbLocation databaseLocation) : base()
        {
            this._connectionStringDto = connectionStringDto;
            this._databaseLocation = databaseLocation;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (_databaseLocation.DatabaseLocation== DatabaseLocation.InMemory)
            {
                optionsBuilder.UseInMemoryDatabase();
            }
            else if (_databaseLocation.DatabaseLocation == DatabaseLocation.SqlServer)
            {
                optionsBuilder.UseSqlServer(this._connectionStringDto.ConnectionString);
            }
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
                .HasKey(t => new { t.PermissionId, t.RoleId });


        }

        public DbSet<PermissionManager.BLL.Models.Permission> Permissions { get; set; }
        public DbSet<PermissionRole<TRole, TKey>> PermissionRoles { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }

    }

    public enum DatabaseLocation
    {
        SqlServer = 1,
        InMemory = 2
    }

    public class DbLocation
    {
        public  DatabaseLocation DatabaseLocation { get; }
        public DbLocation(DatabaseLocation databaseLocation)
        {
            DatabaseLocation = databaseLocation;
            this.DatabaseLocation = databaseLocation;
        }
    }
}