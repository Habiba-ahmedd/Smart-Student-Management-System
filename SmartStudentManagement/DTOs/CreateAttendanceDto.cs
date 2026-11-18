
using System.ComponentModel.DataAnnotations;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.DTOs
{
    public class CreateAttendanceDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }
    }
}

