using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Discount_Server.Controllers
{
	public class UserController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
