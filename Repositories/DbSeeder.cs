using ContactApp.Models;
using ContactApp.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ContactApp.Repositories
{
    public static class DbSeeder
    {
        public static async Task Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (context.Contacts.Any())
            {
                return;
            }
            var seed1 = new List<Contact>
            {
                new Contact("John", "Doe", "john.doe@example.com", "123-456-7890", "ABC Inc.", "Manager", "Initial contact"),
                new Contact("Emily", "Clarke", "emily.clarke@example.com", "212-555-0148", "Nova Tech", "Software Engineer", "Referred by a colleague"),
                new Contact("Mehmet", "Yılmaz", "mehmet.yilmaz@example.com", "532-111-2233", "Yılmaz Holding", "Finance Director", "Met at industry conference"),
                new Contact("Sara", "Ahmed", "sara.ahmed@example.com", "070-9988-7766", "Global Logistics", "Operations Lead", "Follow up next quarter"),
                new Contact("Carlos", "Mendes", "carlos.mendes@example.com", "011-4455-6677", "Sunrise Marketing", "Creative Director", "Interested in partnership"),
            };

            context.Contacts.AddRange(seed1);
            context.SaveChanges();

            //Seed Role
            if (await roleManager.FindByNameAsync("Admin") == null) {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin", NormalizedName = "ADMIN" }); 
            }

            //Seed Admin User
            if (await userManager.FindByNameAsync("admin") == null) { 
                var adminUser = new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "admin1234");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

            }
            
        }
    }

}