
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SmartStudentManagement.Data;
using SmartStudentManagement.DTOs;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GradeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Teacher adds a grade
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddGrade(CreateGradeDto dto)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (dto.Score > dto.MaxScore)
                return BadRequest("Score cannot exceed MaxScore.");

            var course = await _context.Courses
                .FindAsync(dto.CourseId);

            if (course == null)
                return NotFound("Course not found.");

            if (course.TeacherId != teacherId)
                return Forbid();

            var student = await _context.Students
                .FindAsync(dto.StudentId);

            if (student == null)
                return NotFound("Student not found.");

            var isEnrolled = await _context.StudentCourses
                .AnyAsync(sc =>
                    sc.StudentId == dto.StudentId &&
                    sc.CourseId == dto.CourseId);

            if (!isEnrolled)
                return BadRequest("Student is not enrolled in this course.");

            var grade = new Grade
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                TeacherId = teacherId,
                Score = dto.Score,
                MaxScore = dto.MaxScore,
                DateRecorded = DateTime.UtcNow
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            return Ok("Grade added successfully.");
        }

        // Admin views all grades
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllGrades()
        {
            var grades = await _context.Grades
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    StudentId = g.StudentId,
                    StudentName = g.Student.FullName,
                    CourseId = g.CourseId,
                    CourseName = g.Course.CourseName,
                    TeacherName = g.Teacher.FullName,
                    Score = g.Score,
                    MaxScore = g.MaxScore,
                    DateRecorded = g.DateRecorded
                })
                .ToListAsync();

            return Ok(grades);
        }

        // Teacher views grades for their courses
        [HttpGet("my-courses")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherGrades()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var grades = await _context.Grades
                .Where(g => g.TeacherId == teacherId)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    StudentId = g.StudentId,
                    StudentName = g.Student.FullName,
                    CourseId = g.CourseId,
                    CourseName = g.Course.CourseName,
                    TeacherName = g.Teacher.FullName,
                    Score = g.Score,
                    MaxScore = g.MaxScore,
                    DateRecorded = g.DateRecorded
                })
                .ToListAsync();

            return Ok(grades);
        }

        // Student views their own grades
        [HttpGet("my-grades")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyGrades()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var grades = await _context.Grades
                .Where(g => g.Student.ApplicationUserId == userId)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    StudentId = g.StudentId,
                    StudentName = g.Student.FullName,
                    CourseId = g.CourseId,
                    CourseName = g.Course.CourseName,
                    TeacherName = g.Teacher.FullName,
                    Score = g.Score,
                    MaxScore = g.MaxScore,
                    DateRecorded = g.DateRecorded
                })
                .ToListAsync();

            return Ok(grades);
        }

        // Teacher updates a grade in their course
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateGrade(
            int id, UpdateGradeDto dto)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
                return NotFound("Grade not found.");

            if (grade.TeacherId != teacherId)
                return Forbid();

            if (dto.Score > dto.MaxScore)
                return BadRequest("Score cannot exceed MaxScore.");

            grade.Score = dto.Score;
            grade.MaxScore = dto.MaxScore;

            await _context.SaveChangesAsync();

            return Ok("Grade updated successfully.");
        }
    }
}
