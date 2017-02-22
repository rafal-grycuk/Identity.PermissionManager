using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.PermissionManager.BLL.Models
{
    public class PermissionRole<TRole, TKey>
       where TRole : IdentityRole<TKey>
       where TKey : IEquatable<TKey>
    {
        public TKey RoleId { get; set; }

        public int PermissionId { get; set; }

        [ForeignKey("PermissionId")]
        public virtual PermissionManager.BLL.Models.Permission Permission { get; set; }

        [ForeignKey("RoleId")]
        public virtual TRole Role { get; set; }
    }
}
