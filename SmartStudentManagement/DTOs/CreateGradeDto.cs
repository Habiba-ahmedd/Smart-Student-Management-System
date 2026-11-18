
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class CreateGradeDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal MaxScore { get; set; }
    }
}
