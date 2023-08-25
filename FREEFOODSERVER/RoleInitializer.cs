using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FREEFOODSERVER
{
    public class RoleInitializer
    {
        public static async Task<IdentityResult> InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@example.com";
            string adminPassword = "_Aa123456";
            string userEmail = "user@example.com";
            string userPassword = "_Aa123456";
            string companyEmail = "company@example.com";
            string companyPassword = "_Aa123456";
            if (await roleManager.FindByNameAsync(UserRoles.Admin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (await roleManager.FindByNameAsync(UserRoles.User) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            if (await roleManager.FindByNameAsync(UserRoles.Company) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Company));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                Admin admin = new() { Email = adminEmail, UserName = adminEmail, EmailConfirmed = true };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, UserRoles.Admin);
                }
            }
            if (await userManager.FindByNameAsync(userEmail) == null)
            {
                User user = new(){ Email = userEmail, UserName = "Coolname", EmailConfirmed = true, Name = "Cool name" };
                IdentityResult result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }
            }
            if (await userManager.FindByNameAsync(companyEmail) == null)
            {
                Company company = new Company { Email = companyEmail, UserName = "SoulGood", EmailConfirmed = true, Bags = new List<Bag>(), CompanyName = "Soul Good" };
                IdentityResult result = await userManager.CreateAsync(company, companyPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(company, UserRoles.Company);
                }
            }
            return IdentityResult.Success;
        }
    }
}
