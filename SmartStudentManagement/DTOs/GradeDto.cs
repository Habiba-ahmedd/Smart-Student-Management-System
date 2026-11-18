
namespace SmartStudentManagement.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public string TeacherName { get; set; }

        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }

        public DateTime DateRecorded { get; set; }
    }
}
