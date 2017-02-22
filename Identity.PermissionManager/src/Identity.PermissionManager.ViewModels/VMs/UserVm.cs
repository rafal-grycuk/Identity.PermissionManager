using System.Collections.Generic;

namespace Identity.PermissionManager.ViewModels.VMs
{
    public class UserVm
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public IEnumerable<RoleVm> RolesList { get; set; }
        
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
