using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FREEFOODSERVER
{
    public class RoleInitializer
    {
        public static async Task<IdentityResult> InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
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
                User admin = new User { Email = adminEmail, UserName = adminEmail, EmailConfirmed = true };
                admin.UserInfo = new AdminInfo() {BannedCount = 10};
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, UserRoles.Admin);
                }
            }
            if (await userManager.FindByNameAsync(userEmail) == null)
            {
                User user = new User { Email = userEmail, UserName = userEmail, EmailConfirmed = true };
                user.UserInfo = new StandardUserInfo() { number = 1 };
                IdentityResult result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }
            }
            if (await userManager.FindByNameAsync(companyEmail) == null)
            {
                User company = new User { Email = companyEmail, UserName = companyEmail, EmailConfirmed = true };
                company.UserInfo = new CompanyInfo() { Bags = new List<Bag>()};
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
