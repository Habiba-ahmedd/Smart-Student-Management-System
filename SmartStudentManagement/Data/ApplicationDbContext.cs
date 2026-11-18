using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<SchoolClass> Classes { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Attendance> Attendances { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // StudentCourse: prevent deleting related Student or Course
            builder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student
            builder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            builder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne()
                .HasForeignKey<Student>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Student>()
                .HasOne(s => s.SchoolClass)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.SchoolClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course
            builder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade
            builder.Entity<Grade>()
                .Property(g => g.Score)
                .HasPrecision(8, 2);

            builder.Entity<Grade>()
                .Property(g => g.MaxScore)
                .HasPrecision(8, 2);

            builder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany()
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany()
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Attendance
            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany()
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attendance>()
                .HasOne(a => a.Teacher)
                .WithMany()
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    }