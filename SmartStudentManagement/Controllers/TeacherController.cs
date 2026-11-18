
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SmartStudentManagement.Models;
using SmartStudentManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using SmartStudentManagement.Data;
namespace SmartStudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TeacherController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public TeacherController(
     UserManager<ApplicationUser> userManager,
     ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // Get all teachers
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTeachers()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");

            var result = teachers.Select(t => new
            {
                Id = t.Id,
                FullName = t.FullName,
                Email = t.Email
            }).ToList();

            return Ok(result);
        }


        // POST: api/Teacher
        [HttpPost]
        public async Task<IActionResult> CreateTeacher(
            CreateTeacherDto dto)
        {
            var existingUser = await _userManager
                .FindByEmailAsync(dto.Email);

            if (existingUser != null)
                return BadRequest("Email already exists.");

            var teacher = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager
                .CreateAsync(teacher, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager
                .AddToRoleAsync(teacher, "Teacher");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(teacher);
                return BadRequest(roleResult.Errors);
            }

            return Ok(new
            {
                message = "Teacher created successfully.",
                teacherId = teacher.Id,
                fullName = teacher.FullName,
                email = teacher.Email
            });
        }


        // Delete teacher
        [HttpDelete("{teacherId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeacher(string teacherId)
        {
            var teacher = await _userManager.FindByIdAsync(teacherId);

            if (teacher == null)
                return NotFound("Teacher not found.");

            var isAssignedToCourses = await _context.Courses
                .AnyAsync(c => c.TeacherId == teacherId);

            var hasGrades = await _context.Grades
                .AnyAsync(g => g.TeacherId == teacherId);

            var hasAttendance = await _context.Attendances
                .AnyAsync(a => a.TeacherId == teacherId);

            if (isAssignedToCourses || hasGrades || hasAttendance)
            {
                return BadRequest(
                    "Cannot delete teacher because they have related courses, grades, or attendance records.");
            }

            var result = await _userManager.DeleteAsync(teacher);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Teacher deleted successfully.");
        }

        // Get teacher by ID
        [HttpGet("{teacherId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTeacherById(string teacherId)
        {
            var teacher = await _userManager.FindByIdAsync(teacherId);

            if (teacher == null)
                return NotFound("Teacher not found.");

            var isTeacher = await _userManager.IsInRoleAsync(
                teacher, "Teacher");

            if (!isTeacher)
                return NotFound("Teacher not found.");

            return Ok(new
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                Email = teacher.Email
            });
        }


    }
}
