namespace SmartStudentManagement.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string CourseCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string TeacherId { get; set; } = string.Empty;

        public ApplicationUser Teacher { get; set; } = null!;

        public ICollection<StudentCourse> StudentCourses { get; set; }
            = new List<StudentCourse>();
    }
}