using IdentityServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebServer.Data;

namespace IdentityServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	
	public class IdentityController : Controller
	{

		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ApplicationDbContext _dbContext;
		private readonly IConfiguration _configuration;
		public IdentityController(UserManager<AppUser> userManager,
			RoleManager<IdentityRole> roleManager,
			SignInManager<AppUser> signInManager,
			ApplicationDbContext applicationDbContext,
			IConfiguration configuration
			)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_dbContext = applicationDbContext;
			_configuration = configuration;
		}

		[HttpPost("Login")]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login([FromBody] LoginViewModel loginUser)
		{
			//if (ModelState.IsValid)
			//{
			//	var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, true, false);
			//	return IdentityResult.Success;
			//}
			//else
			//{
			//	return IdentityResult.Failed(new IdentityError() { Code = BadRequest().ToString(), Description = "Неверная модель"}) ;
			//}

			var user = await _userManager.FindByEmailAsync(loginUser.Email);
			if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user);

				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				};

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var token = GetToken(authClaims);
				
				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expiration = token.ValidTo
				});
			}
			return Unauthorized();
		}

		[NonAction]
		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
				);

			return token;
		}
	}
}
