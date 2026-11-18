namespace SmartStudentManagement.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;

        public string TeacherId { get; set; } = string.Empty;

        public ApplicationUser Teacher { get; set; } = null!;

        public decimal Score { get; set; }

        public decimal MaxScore { get; set; }

        public DateTime DateRecorded { get; set; }
    }
}