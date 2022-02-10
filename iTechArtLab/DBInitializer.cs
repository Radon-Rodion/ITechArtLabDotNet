using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace iTechArtLab
{
    public class DBInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            string adminEmail = "admin@gmail.com";
            string password = "_Aa123456";
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new IdentityUser { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    await userManager.ConfirmEmailAsync(admin, await userManager.GenerateEmailConfirmationTokenAsync(admin));
                }
                else Log.Logger.Warning("Admin in DB not initialized");
            }
        }

        private static async void DeleteAllAsync (UserManager<IdentityUser> userManager)
        {
            var usersArr = userManager.Users.ToArray();
            foreach(var user in usersArr)
            {
                await userManager.DeleteAsync(user);
            }
        }
    }
}
