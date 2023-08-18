using IdentityServer.Models.Roles;
using IdentityServer.Models.Users;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebServer.Areas.Identity.Pages.Account;
using WebServer.Data;

namespace WebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IConfiguration _configuration;
		public AdminController(UserManager<AppUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IConfiguration configuration,
            SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_signInManager = signInManager;
        }

		[HttpGet("Login")]
		[AllowAnonymous]
		public ViewResult GetLogin()
		{
			return View("~/Views/Admin/Login.cshtml");
		}

		[HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> PostLogin([FromForm]LoginViewModel model, string? ReturnUrl)
		{
            if (ModelState.IsValid)
            {
				var result =
					await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
						return LocalRedirect("~/api/Admin/Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);

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

		[HttpGet("Home")]
		//[Authorize(Roles = UserRoles.Admin)]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Home(string? ReturnUrl)
		{
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
			string users = string.Empty;
			_userManager.Users.ToList().ForEach(obj => users += obj.ToString());
			return Content($"{users}, {role}");
		}
	}
}
