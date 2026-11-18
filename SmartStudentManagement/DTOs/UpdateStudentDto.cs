
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class UpdateStudentDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        public int? SchoolClassId { get; set; }
    }
}
