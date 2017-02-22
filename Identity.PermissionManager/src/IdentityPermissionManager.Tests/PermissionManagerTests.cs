//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Identity.Permission.Manager.BLL.Models;
//using Identity.Permission.Manager.Core.PermissionManager;
//using Identity.Permission.Manager.DAL;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Xunit;

//namespace Identity.Permission.Manager.Tests
//{
//    public class PermissionManagerTests
//    {
//        //private readonly PermissionManager<User, Role, int> permissionManager;
//        private readonly PermissionManagerDbContext<User, Role, int> context;
//        public PermissionManagerTests()
//        {
//            var mockSet = new Mock<DbSet<User>>();
//            var mockContext = new Mock<PermissionManagerDbContext<User, Role, int>>();
//            mockContext.Setup(m => m.Users).Returns(mockSet.Object);
//            this.context = mockContext.Object;

//        }

//        [Fact]
//        public void AddPermissionTest()
//        {
//            var users = context.Users.ToList();
//            Assert.True(false);
//        }
//    }
//}
