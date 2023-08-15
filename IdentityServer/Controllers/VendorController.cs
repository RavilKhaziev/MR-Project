using IdentityServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class VendorController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		public VendorController(UserManager<AppUser> userManager,
			RoleManager<IdentityRole> roleManager,
			SignInManager<AppUser> signInManager
			)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		[HttpPost("Registration")]
		//[ValidateAntiForgeryToken]
		public async Task<IdentityResult> Registration([FromBody] VendorRegistrationModelView newVendor)
		{
			if (ModelState.IsValid)
			{
				return await _userManager.CreateAsync(await BuildVendorAsync(newVendor), newVendor.Password);
			}
			else
			{
				return IdentityResult.Failed();
			}
		}

		[NonAction]
		protected async Task<AppUser> BuildVendorAsync(VendorRegistrationModelView vendor)
		{
			return new AppUser
			{
				UserName = vendor.CompanyName,
				Email = vendor.CompanyEmail,
				PhoneNumber = vendor.PhoneNumber,
				Discritption = vendor.Discritption,
				Location = vendor.Location
			}; 
		}


	}
}
