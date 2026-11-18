
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartStudentManagement.Data;
using SmartStudentManagement.DTOs;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    CourseName = c.CourseName,
                    CourseCode = c.CourseCode,
                    Description = c.Description,
                    TeacherId = c.TeacherId,
                    TeacherName = c.Teacher.FullName
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/Course/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Where(c => c.Id == id)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    CourseName = c.CourseName,
                    CourseCode = c.CourseCode,
                    Description = c.Description,
                    TeacherId = c.TeacherId,
                    TeacherName = c.Teacher.FullName
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound("Course not found.");

            return Ok(course);
        }

        // POST: api/Course
        [HttpPost]
        public async Task<IActionResult> CreateCourse(
            CreateCourseDto dto)
        {
            var existingCourse = await _context.Courses
                .AnyAsync(c => c.CourseCode == dto.CourseCode);

            if (existingCourse)
                return BadRequest("Course code already exists.");

            var teacher = await _userManager
                .FindByIdAsync(dto.TeacherId);

            if (teacher == null ||
                !await _userManager.IsInRoleAsync(teacher, "Teacher"))
            {
                return BadRequest("Invalid teacher ID.");
            }

            var course = new Course
            {
                CourseName = dto.CourseName,
                CourseCode = dto.CourseCode,
                Description = dto.Description,
                TeacherId = teacher.Id
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Course created successfully.",
                courseId = course.Id
            });
        }

        // PUT: api/Course/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(
            int id, UpdateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound("Course not found.");

            var duplicateCode = await _context.Courses
                .AnyAsync(c =>
                    c.CourseCode == dto.CourseCode &&
                    c.Id != id);

            if (duplicateCode)
                return BadRequest("Course code already exists.");

            var teacher = await _userManager
                .FindByIdAsync(dto.TeacherId);

            if (teacher == null ||
                !await _userManager.IsInRoleAsync(teacher, "Teacher"))
            {
                return BadRequest("Invalid teacher ID.");
            }

            course.CourseName = dto.CourseName;
            course.CourseCode = dto.CourseCode;
            course.Description = dto.Description;
            course.TeacherId = teacher.Id;

            await _context.SaveChangesAsync();

            return Ok("Course updated successfully.");
        }

        // DELETE: api/Course/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound("Course not found.");

            var hasStudents = await _context.StudentCourses
                .AnyAsync(sc => sc.CourseId == id);

            var hasGrades = await _context.Grades
                .AnyAsync(g => g.CourseId == id);

            var hasAttendance = await _context.Attendances
                .AnyAsync(a => a.CourseId == id);

            if (hasStudents || hasGrades || hasAttendance)
            {
                return BadRequest(
                    "Cannot delete course because it has related records.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok("Course deleted successfully.");
        }
    }
}
