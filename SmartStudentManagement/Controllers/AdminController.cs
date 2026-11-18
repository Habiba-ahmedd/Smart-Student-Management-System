
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
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Get all users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        // Update user role

        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(
            string userId,
            UpdateUserRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            var validRoles = new[] { "Admin", "Teacher", "Student" };

            if (string.IsNullOrWhiteSpace(dto.Role) ||
                !validRoles.Contains(dto.Role))
                return BadRequest("Invalid role.");

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return BadRequest("Role does not exist.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Count == 1 &&
                currentRoles.Contains(dto.Role))
                return Ok("User already has this role.");

            if (currentRoles.Contains("Student"))
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                if (student != null)
                {
                    var hasEnrollments = await _context.StudentCourses
                        .AnyAsync(sc => sc.StudentId == student.Id);

                    var hasGrades = await _context.Grades
                        .AnyAsync(g => g.StudentId == student.Id);

                    var hasAttendance = await _context.Attendances
                        .AnyAsync(a => a.StudentId == student.Id);

                    if (hasEnrollments || hasGrades || hasAttendance)
                        return BadRequest(
                            "Cannot change Student role because the student has related academic records.");

                    return BadRequest(
                        "Cannot change Student role while a Student profile exists.");
                }
            }

            if (currentRoles.Contains("Teacher"))
            {
                var hasCourses = await _context.Courses
                    .AnyAsync(c => c.TeacherId == userId);

                var hasGrades = await _context.Grades
                    .AnyAsync(g => g.TeacherId == userId);

                var hasAttendance = await _context.Attendances
                    .AnyAsync(a => a.TeacherId == userId);

                if (hasCourses || hasGrades || hasAttendance)
                    return BadRequest(
                        "Cannot change Teacher role because the teacher has related academic records.");
            }

            var removeResult = await _userManager.RemoveFromRolesAsync(
                user, currentRoles);

            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            var addResult = await _userManager.AddToRoleAsync(
                user, dto.Role);

            if (!addResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, currentRoles);
                return BadRequest(addResult.Errors);
            }

            return Ok("User role updated successfully.");
        }


        // Get system statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var totalUsers = await _userManager.Users.CountAsync();

            var totalStudents = await _context.Students.CountAsync();

            var totalTeachers = await _userManager.GetUsersInRoleAsync("Teacher");

            var totalCourses = await _context.Courses.CountAsync();

            var totalClasses = await _context.Classes.CountAsync();

            var totalGrades = await _context.Grades.CountAsync();

            var totalAttendance = await _context.Attendances.CountAsync();

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers.Count,
                TotalCourses = totalCourses,
                TotalClasses = totalClasses,
                TotalGrades = totalGrades,
                TotalAttendanceRecords = totalAttendance
            });
        }
    }
}
