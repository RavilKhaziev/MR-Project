using IdentityServer.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models.Roles
{
	public class RoleInitializer
	{
		public static async Task InitializeAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			string adminEmail = "admin@gmail.com";
			string password = "_Aa123456";
			if (await roleManager.FindByNameAsync(UserRoles.Admin) == null)
			{
				await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			}
			if (await roleManager.FindByNameAsync(UserRoles.User) == null)
			{
				await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
			}
			if (await roleManager.FindByNameAsync(UserRoles.Vendor) == null)
			{
				await roleManager.CreateAsync(new IdentityRole(UserRoles.Vendor));
			}
			if (await userManager.FindByNameAsync(adminEmail) == null)
			{
				AppUser admin = new AppUser { Email = adminEmail, UserName = adminEmail, EmailConfirmed = true };
				IdentityResult result = await userManager.CreateAsync(admin, password);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(admin, UserRoles.Admin);
				}
			}

		}
	}
}
