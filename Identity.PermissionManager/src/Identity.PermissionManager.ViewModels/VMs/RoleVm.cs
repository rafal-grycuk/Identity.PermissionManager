using System.Collections.Generic;

namespace Identity.PermissionManager.ViewModels.VMs
{
    public class RoleVm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> Permissions { get; set; }
    }
}
