using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FREEFOODSERVER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AdminController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AdminController> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult GETLogin(string? returnUrl)
        {
            return View("Login");
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> POSTLogin([FromForm]LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (result.Succeeded) {
                        if (string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else return RedirectToAction("Index", "Admin");

                    }
                    else return RedirectToAction("Login", "Admin");
                }
            }
            return RedirectToAction("Login", "Admin");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> POSTLogout(string? returnUrl)
        {
            await _signInManager.SignOutAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return View("Login");
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            List<UserViewModel> model = new();
            foreach (var user in _userManager.Users)
            {
                model.Add(new UserViewModel { user = user });
            }
            foreach (var user in model)
            {
                user.roles = await _userManager.GetRolesAsync(user.user);
            }
            
            return View("Index", model);
        }

        [HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}