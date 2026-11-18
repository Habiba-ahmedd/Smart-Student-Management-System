
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class CreateTeacherDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
