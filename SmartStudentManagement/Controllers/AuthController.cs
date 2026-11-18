
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartStudentManagement.Data;
using SmartStudentManagement.DTOs;
using SmartStudentManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace SmartStudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.FullName,
                user.Email,
                Roles = roles
            });
        }

        // REGISTER

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                return BadRequest("Email already exists.");

            var existingStudent = await _context.Students
                .AnyAsync(s => s.StudentNumber == dto.StudentNumber);

            if (existingStudent)
                return BadRequest("Student number already exists.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = dto.Email.Trim(),
                    Email = dto.Email.Trim(),
                    FullName = dto.FullName.Trim()
                };

                var result = await _userManager.CreateAsync(
                    user, dto.Password);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(result.Errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(
                    user, "Student");

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(roleResult.Errors);
                }

                var student = new Student
                {
                    StudentNumber = dto.StudentNumber.Trim(),
                    FullName = dto.FullName.Trim(),
                    Email = dto.Email.Trim(),
                    EnrollmentDate = DateTime.UtcNow,
                    ApplicationUserId = user.Id
                };

                _context.Students.Add(student);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Student registered successfully.",
                    studentId = student.Id
                });
            }
            catch
            {
                await transaction.RollbackAsync();

                return StatusCode(
                    500,
                    "An error occurred during registration.");
            }
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null ||
                !await _userManager.CheckPasswordAsync(
                    user, dto.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(
                        _configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return Ok(new
            {
                message = "Login successful.",
                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                expiration = token.ValidTo,
                roles = roles
            });
        }
    }
}
