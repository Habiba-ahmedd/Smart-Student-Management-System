
using System.ComponentModel.DataAnnotations;

namespace SmartStudentManagement.DTOs
{
    public class UpdateGradeDto
    {
        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal MaxScore { get; set; }
    }
}
