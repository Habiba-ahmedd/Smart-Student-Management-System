
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class CreateClassDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
