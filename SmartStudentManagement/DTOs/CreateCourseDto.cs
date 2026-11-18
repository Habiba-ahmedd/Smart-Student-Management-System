
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public string CourseCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string TeacherId { get; set; } = string.Empty;
    }
}
