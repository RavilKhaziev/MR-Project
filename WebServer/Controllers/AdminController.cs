using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebServer.Data;

namespace WebServer.Controllers
{
	public class AdminController : Controller
	{
		UserManager<IdentityUser> _userManager;
		RoleManager<IdentityRole> _roleManager;
		public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public IActionResult Index()
		{
			var users = _userManager.Users.ToList();

			return View(users);
		}
	}
}
