using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rad302feWebAPI2025.DataLayer
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            // Check if the database already has users
            if (userManager.Users.Any())
            {
                return; // Database has been seeded
            }

            // Create roles
            string[] roles = new string[] { "Movie Editor", "Employee", "Super User" };
            
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create users
            var users = new List<(ApplicationUser user, string password, string role)>
            {
                (
                    new ApplicationUser
                    {
                        UserName = "bbloggs@atu.ie",
                        Email = "bbloggs@atu.ie",
                        FirstName = "Bob",
                        SecondName = "Bloggs",
                        EmailConfirmed = true
                    },
                    "Password$1",
                    "Movie Editor"
                ),
                (
                    new ApplicationUser
                    {
                        UserName = "fspectre@atu.ie",
                        Email = "fspectre@atu.ie",
                        FirstName = "Fred",
                        SecondName = "Spectre",
                        EmailConfirmed = true
                    },
                    "Password$1",
                    "Employee"
                ),
                (
                    new ApplicationUser
                    {
                        UserName = "mruddy@atu.ie",
                        Email = "mruddy@atu.ie",
                        FirstName = "Martha",
                        SecondName = "Ruddy",
                        EmailConfirmed = true
                    },
                    "Password$1",
                    "Super User"
                )
            };

            foreach (var (user, password, role) in users)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
