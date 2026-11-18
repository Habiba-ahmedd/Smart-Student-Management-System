namespace SmartStudentManagement.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late
    }

    public class Attendance
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;

        public string TeacherId { get; set; } = string.Empty;

        public ApplicationUser Teacher { get; set; } = null!;

        public DateTime AttendanceDate { get; set; }

        public AttendanceStatus Status { get; set; }
    }
}