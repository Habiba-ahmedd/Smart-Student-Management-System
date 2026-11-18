
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
    public class ClassController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new class
        [HttpPost]
        public async Task<IActionResult> CreateClass(CreateClassDto dto)
        {
            var schoolClass = new SchoolClass
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Classes.Add(schoolClass);
            await _context.SaveChangesAsync();

            return Ok("Class created successfully.");
        }

        // Get all classes
        [HttpGet]
        public async Task<IActionResult> GetAllClasses()
        {
            var classes = await _context.Classes
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    StudentsCount = c.Students.Count()
                })
                .ToListAsync();

            return Ok(classes);
        }

        // Get class by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(int id)
        {
            var schoolClass = await _context.Classes
                .Where(c => c.Id == id)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    StudentsCount = c.Students.Count()
                })
                .FirstOrDefaultAsync();

            if (schoolClass == null)
                return NotFound("Class not found.");

            return Ok(schoolClass);
        }

        // Update class
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(
            int id, UpdateClassDto dto)
        {
            var schoolClass = await _context.Classes.FindAsync(id);

            if (schoolClass == null)
                return NotFound("Class not found.");

            schoolClass.Name = dto.Name;
            schoolClass.Description = dto.Description;

            await _context.SaveChangesAsync();

            return Ok("Class updated successfully.");
        }

        // Delete class
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var schoolClass = await _context.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (schoolClass == null)
                return NotFound("Class not found.");

            if (schoolClass.Students.Any())
                return BadRequest(
                    "Cannot delete a class that has students assigned to it.");

            _context.Classes.Remove(schoolClass);
            await _context.SaveChangesAsync();

            return Ok("Class deleted successfully.");
        }

        // Get students in a specific class
        [HttpGet("{id}/students")]
        public async Task<IActionResult> GetClassStudents(int id)
        {
            var schoolClass = await _context.Classes
                .AnyAsync(c => c.Id == id);

            if (!schoolClass)
                return NotFound("Class not found.");

            var students = await _context.Students
                .Where(s => s.SchoolClassId == id)
                .Select(s => new
                {
                    s.Id,
                    s.StudentNumber,
                    s.FullName,
                    s.Email
                })
                .ToListAsync();

            return Ok(students);
        }
    }
}
