
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class CreateStudentDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string StudentNumber { get; set; } = string.Empty;

        public int? SchoolClassId { get; set; }
    }
}
