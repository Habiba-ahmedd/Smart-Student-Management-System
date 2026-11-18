namespace SmartStudentManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string StudentNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;

        public ApplicationUser ApplicationUser { get; set; } = null!;

        public int? SchoolClassId { get; set; }

        public SchoolClass? SchoolClass { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
            = new List<StudentCourse>();
    }
}