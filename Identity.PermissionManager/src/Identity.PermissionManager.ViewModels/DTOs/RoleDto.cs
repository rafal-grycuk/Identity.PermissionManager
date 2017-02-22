using System.ComponentModel.DataAnnotations;

namespace Identity.PermissionManager.ViewModels.DTOs
{
    public class RoleDto
    {
        [Required]
        public string Name { get; set; }
    }
}
