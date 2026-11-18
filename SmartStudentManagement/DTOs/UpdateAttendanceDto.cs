
using System.ComponentModel.DataAnnotations;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.DTOs
{
    public class UpdateAttendanceDto
    {
        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }
    }
}
