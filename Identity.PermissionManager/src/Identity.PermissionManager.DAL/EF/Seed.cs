using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Core.Interfaces.UoW;
using Identity.PermissionManager.BLL.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.PermissionManager.DAL.EF
{
    public static class Seed
    {
        public static void SeedData(IUnitOfWork uow, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            Permission read = new Permission()
            {
                Name = "Read"
            };
            uow.Repository<Permission>().Add(read);
            Permission write = new Permission()
            {
                Name = "Write"
            };
            uow.Repository<Permission>().Add(write);

            PermissionGroup group1 = new PermissionGroup()
            {
                GroupName = "group1",
                Permissions = new List<Permission>()
            };

            group1.Permissions.Add(read);
            group1.Permissions.Add(write);
            uow.Repository<PermissionGroup>().Add(group1);

            uow.Save();

            User user1 = new User()
            {
                UserName = "jnowak",
                FirstName = "Jan",
                LastName = "Nowak",
                Email = "jnowak@test.test"
            };
            userManager.CreateAsync(user1, "Test12345^").Wait();
            User user2 = new User()
            {
                UserName = "wkowalski",
                FirstName = "Wojciech",
                LastName = "Kowalski",
                Email = "wkowalski@test.test"
            };
            userManager.CreateAsync(user2, "Test12345^").Wait();

            Role adminRole = new Role()
            {
                Name = "Admin"
            };

            Role userRole = new Role()
            {
                Name = "User"
            };

             roleManager.CreateAsync(adminRole).Wait();
             roleManager.CreateAsync(userRole).Wait();

             userManager.AddToRoleAsync(user1, adminRole.Name).Wait();
             userManager.AddToRoleAsync(user2, userRole.Name).Wait();


        }
    }
}
