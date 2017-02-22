using System.Collections.Generic;

namespace Identity.PermissionManager.ViewModels.VMs
{
    public class PermissionVm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> Roles { get; set; }

        public string PermissionGroup { get; set; }
        
        public int? PermissionGroupId { get; set; }
    }
}
