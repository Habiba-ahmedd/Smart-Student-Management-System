using Microsoft.AspNetCore.Identity;

namespace SmartStudentManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}