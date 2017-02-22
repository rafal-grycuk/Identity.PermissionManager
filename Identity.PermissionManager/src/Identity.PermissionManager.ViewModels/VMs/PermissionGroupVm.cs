using System.Collections.Generic;

namespace Identity.PermissionManager.ViewModels.VMs
{
    public class PermissionGroupVm
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public ICollection<PermissionVm> Permissions { get; set; }
    }
}
