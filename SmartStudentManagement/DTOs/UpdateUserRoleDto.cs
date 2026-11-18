
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
