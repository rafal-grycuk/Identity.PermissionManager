using System.ComponentModel.DataAnnotations;

namespace Identity.PermissionManager.ViewModels.DTOs
{
    public class PermissionDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? PermissionGroupId { get; set; }
    }
}
