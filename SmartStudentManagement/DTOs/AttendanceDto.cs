
using SmartStudentManagement.Models;

namespace SmartStudentManagement.DTOs
{
    public class AttendanceDto
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public string TeacherName { get; set; }

        public DateTime AttendanceDate { get; set; }

        public AttendanceStatus Status { get; set; }
    }
}
