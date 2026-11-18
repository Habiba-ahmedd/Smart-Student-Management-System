
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
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Teacher marks attendance
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> MarkAttendance(
            CreateAttendanceDto dto)
        {
            var teacherId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                return BadRequest(
                    "Student is not enrolled in this course.");

            var date = dto.AttendanceDate.Date;

            var alreadyMarked = await _context.Attendances
                .AnyAsync(a =>
                    a.StudentId == dto.StudentId &&
                    a.CourseId == dto.CourseId &&
                    a.AttendanceDate.Date == date);

            if (alreadyMarked)
                return BadRequest(
                    "Attendance is already marked for this student on this date.");

            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status))
                return BadRequest("Invalid attendance status.");

            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                TeacherId = teacherId,
                AttendanceDate = date,
                Status = dto.Status
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok("Attendance marked successfully.");
        }

        // Admin views all attendance records
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAttendance()
        {
            var records = await _context.Attendances
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    CourseId = a.CourseId,
                    CourseName = a.Course.CourseName,
                    TeacherName = a.Teacher.FullName,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(records);
        }

        // Teacher views attendance for their courses
        [HttpGet("my-courses")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherAttendance()
        {
            var teacherId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var records = await _context.Attendances
                .Where(a => a.TeacherId == teacherId)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    CourseId = a.CourseId,
                    CourseName = a.Course.CourseName,
                    TeacherName = a.Teacher.FullName,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(records);
        }

        // Student views their own attendance
        [HttpGet("my-attendance")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAttendance()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var records = await _context.Attendances
                .Where(a =>
                    a.Student.ApplicationUserId == userId)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    CourseId = a.CourseId,
                    CourseName = a.Course.CourseName,
                    TeacherName = a.Teacher.FullName,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(records);
        }

        // Teacher updates attendance
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateAttendance(
            int id, UpdateAttendanceDto dto)
        {
            var teacherId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attendance = await _context.Attendances
                .FindAsync(id);

            if (attendance == null)
                return NotFound("Attendance record not found.");

            if (attendance.TeacherId != teacherId)
                return Forbid();

            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status))
                return BadRequest("Invalid attendance status.");

            attendance.AttendanceDate = dto.AttendanceDate.Date;
            attendance.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok("Attendance updated successfully.");
        }
    }
}
