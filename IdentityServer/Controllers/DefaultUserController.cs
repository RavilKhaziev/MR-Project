using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using IdentityServer4;
using IdentityServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using WebServer.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class DefaultUserController : Controller
	{

		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ApplicationDbContext _dbContext;
		public DefaultUserController(UserManager<AppUser> userManager,
			RoleManager<IdentityRole> roleManager,
			SignInManager<AppUser> signInManager,
			ApplicationDbContext applicationDbContext
			)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_dbContext = applicationDbContext;
			
		}

		[HttpPost("Registration")]
		//[ValidateAntiForgeryToken]
		public async Task<IdentityResult> Registration([FromBody]DefaultUserRegistrationViewModel newUser)
		{
			if (ModelState.IsValid)
			{
				return await _userManager.CreateAsync(await BuildDefaultUserAsync(newUser), newUser.Password);
			}
			else
			{
				return IdentityResult.Failed();
			}
		}

		[NonAction]
		protected async Task<AppUser> BuildDefaultUserAsync(DefaultUserRegistrationViewModel user)
		{
			return new AppUser
			{
				UserName = user.Name,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
			};
		}

	}
}
