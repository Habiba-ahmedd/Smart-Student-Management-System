
namespace SmartStudentManagement.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }

        public string StudentNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }

        public int? SchoolClassId { get; set; }
    }
}
