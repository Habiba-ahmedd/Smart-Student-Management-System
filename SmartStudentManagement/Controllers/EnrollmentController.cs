
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartStudentManagement.Data;
using SmartStudentManagement.DTOs;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Enroll a student in a course
        [HttpPost]
        public async Task<IActionResult> EnrollStudent(EnrollStudentDto dto)
        {
            var student = await _context.Students
                .FindAsync(dto.StudentId);

            if (student == null)
                return NotFound("Student not found.");

            var course = await _context.Courses
                .FindAsync(dto.CourseId);

            if (course == null)
                return NotFound("Course not found.");

            var alreadyEnrolled = await _context.StudentCourses
                .AnyAsync(sc =>
                    sc.StudentId == dto.StudentId &&
                    sc.CourseId == dto.CourseId);

            if (alreadyEnrolled)
                return BadRequest("Student is already enrolled in this course.");

            var enrollment = new StudentCourse
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId
            };

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Student enrolled successfully.");
        }

        // Get all students enrolled in a specific course
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists)
                return NotFound("Course not found.");

            var students = await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Select(sc => new
                {
                    sc.Student.Id,
                    sc.Student.StudentNumber,
                    sc.Student.FullName,
                    sc.Student.Email
                })
                .ToListAsync();

            return Ok(students);
        }

        // Get all courses for a specific student
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentCourses(int studentId)
        {
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == studentId);

            if (!studentExists)
                return NotFound("Student not found.");

            var courses = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new
                {
                    sc.Course.Id,
                    sc.Course.CourseName,
                    sc.Course.CourseCode,
                    sc.Course.Teacher.FullName
                })
                .ToListAsync();

            return Ok(courses);
        }

        // Remove a student from a course
        [HttpDelete]
        public async Task<IActionResult> RemoveEnrollment(EnrollStudentDto dto)
        {
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc =>
                    sc.StudentId == dto.StudentId &&
                    sc.CourseId == dto.CourseId);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Student removed from course successfully.");
        }
    }
}
