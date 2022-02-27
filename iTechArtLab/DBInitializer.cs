using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;

namespace iTechArtLab
{
    public class DBInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            foreach(Roles role in Enum.GetValues(typeof(Roles)))
            {
                if (await roleManager.FindByNameAsync(Role.Name(role)) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(Role.Name(role)));
                }
            }

            string adminEmail = "admin@gmail.com";
            string password = "_Aa123456";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Role.Name(Roles.Admin));
                    await userManager.ConfirmEmailAsync(admin, await userManager.GenerateEmailConfirmationTokenAsync(admin));
                }
                else Log.Logger.Warning("Admin in DB not initialized");
            }
        }

        private static async void DeleteAllAsync (UserManager<User> userManager)
        {
            Thread.Sleep(300);
            var usersArr = userManager.Users.ToArray();
            foreach(var user in usersArr)
            {
                await userManager.DeleteAsync(user);
            }
        }
    }
}
