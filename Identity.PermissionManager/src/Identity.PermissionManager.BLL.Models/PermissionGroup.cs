using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Identity.PermissionManager.BLL.Models
{
    public class PermissionGroup
    {
        [Key]
        public int Id { get; set; }

        public string GroupName { get; set; }

        public ICollection<PermissionManager.BLL.Models.Permission> Permissions { get; set; }
    }
}
