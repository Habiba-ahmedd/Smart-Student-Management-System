
using Microsoft.AspNetCore.Identity;
using SmartStudentManagement.Models;

namespace SmartStudentManagement.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdmin(
            IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "admin@smartstudent.com";
            string adminPassword = "Admin@12345";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("Admin"));
            }

            var admin = await userManager
                .FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                var result = await userManager
                    .CreateAsync(admin, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ",
                        result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
