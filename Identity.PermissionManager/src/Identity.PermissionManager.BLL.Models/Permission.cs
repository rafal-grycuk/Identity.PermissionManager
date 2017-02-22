using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.PermissionManager.BLL.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<PermissionRole<Role, int>> PermissionRoles { get; set; }

        public virtual PermissionGroup PermissionGroup { get; set; }

        [ForeignKey("PermissionGroup")]
        public int? PermissionGroupId { get; set; }
    }
}
