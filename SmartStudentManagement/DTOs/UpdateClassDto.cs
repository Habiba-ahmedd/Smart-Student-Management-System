
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class UpdateClassDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
