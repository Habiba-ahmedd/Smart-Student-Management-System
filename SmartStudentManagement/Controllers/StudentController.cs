
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartStudentManagement.Data;
using SmartStudentManagement.DTOs;
using Microsoft.AspNetCore.Identity;
using SmartStudentManagement.Models;
namespace SmartStudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentController(
     ApplicationDbContext context,
     UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: api/Student
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FullName = s.FullName,
                    Email = s.Email,
                    EnrollmentDate = s.EnrollmentDate,
                    SchoolClassId = s.SchoolClassId
                })
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/Student/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _context.Students
                .Where(s => s.Id == id)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FullName = s.FullName,
                    Email = s.Email,
                    EnrollmentDate = s.EnrollmentDate,
                    SchoolClassId = s.SchoolClassId
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound("Student not found.");

            return Ok(student);
        }

        // PUT: api/Student/1

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id, UpdateStudentDto dto)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound("Student not found.");

            // Check duplicate student number
            var duplicateNumber = await _context.Students
                .AnyAsync(s =>
                    s.StudentNumber == dto.StudentNumber &&
                    s.Id != id);

            if (duplicateNumber)
                return BadRequest("Student number already exists.");

            // Validate class ID
            if (dto.SchoolClassId.HasValue)
            {
                var classExists = await _context.Classes
                    .AnyAsync(c => c.Id == dto.SchoolClassId.Value);

                if (!classExists)
                    return BadRequest("Class not found.");
            }

            // Find associated Identity user
            var user = await _userManager.FindByIdAsync(
                student.ApplicationUserId);

            if (user == null)
                return NotFound("Student account not found.");

            // Update student data
            student.FullName = dto.FullName;
            student.StudentNumber = dto.StudentNumber;
            student.SchoolClassId = dto.SchoolClassId;

            // Keep Identity account name synchronized
            user.FullName = dto.FullName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _context.SaveChangesAsync();

            return Ok("Student updated successfully.");
        }


        // DELETE: api/Student/1



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound("Student not found.");

            var user = await _userManager.FindByIdAsync(
                student.ApplicationUserId);

            if (user == null)
                return NotFound("Student account not found.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var hasEnrollments = await _context.StudentCourses
                    .AnyAsync(sc => sc.StudentId == id);

                var hasGrades = await _context.Grades
                    .AnyAsync(g => g.StudentId == id);

                var hasAttendance = await _context.Attendances
                    .AnyAsync(a => a.StudentId == id);

                if (hasEnrollments || hasGrades || hasAttendance)
                {
                    await transaction.RollbackAsync();

                    return BadRequest(
                        "Cannot delete student because they have related enrollments, grades, or attendance records.");
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(result.Errors);
                }

                await transaction.CommitAsync();

                return Ok("Student deleted successfully.");
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                return Conflict(
                    "Cannot delete student because related records exist.");
            }
            catch
            {
                await transaction.RollbackAsync();

                return StatusCode(
                    500,
                    "An error occurred while deleting the student.");
            }
        }





        [HttpPost]
        public async Task<IActionResult> CreateStudent(CreateStudentDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                return BadRequest("Email already exists.");

            var existingStudent = await _context.Students
                .AnyAsync(s => s.StudentNumber == dto.StudentNumber);

            if (existingStudent)
                return BadRequest("Student number already exists.");

            if (dto.SchoolClassId.HasValue)
            {
                var classExists = await _context.Classes
                    .AnyAsync(c => c.Id == dto.SchoolClassId.Value);

                if (!classExists)
                    return BadRequest("Class not found.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Student");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(roleResult.Errors);
            }

            var student = new Student
            {
                FullName = dto.FullName,
                Email = dto.Email,
                StudentNumber = dto.StudentNumber,
                EnrollmentDate = DateTime.UtcNow,
                ApplicationUserId = user.Id,
                SchoolClassId = dto.SchoolClassId
            };

            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            return Ok(new
            {
                message = "Student created successfully.",
                studentId = student.Id
            });
        }

    }
}
