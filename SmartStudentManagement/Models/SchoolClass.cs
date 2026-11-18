namespace SmartStudentManagement.Models
{
    public class SchoolClass
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Student> Students { get; set; }
            = new List<Student>();
    }
}