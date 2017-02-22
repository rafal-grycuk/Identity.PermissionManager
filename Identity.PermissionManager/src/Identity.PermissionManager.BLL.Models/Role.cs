using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.PermissionManager.BLL.Models
{
    public class Role : IdentityRole<int>
    {
        public ICollection<PermissionRole<Role, int>> PermissionRoles { get; set; }
        public Role() { }
        public Role(string name) { this.Name = name; }
    }
}

