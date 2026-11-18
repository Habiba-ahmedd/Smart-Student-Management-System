
namespace SmartStudentManagement.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string CourseCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string TeacherId { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;
    }
}
