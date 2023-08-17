using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.NSUser;
using FREEFOODSERVER.Models.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FREEFOODSERVER.Controllers
{
    [ApiController]
    [Route("api/User")]
    [Authorize(Roles = UserRoles.User)]
    public class SUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public SUserController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task<IActionResult> POSTRegistration([FromBody] UserRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User {Email = model.Email,
                    UserName = model.Name,
                    EmailConfirmed = true,
                    UserInfo = new StandardUserInfo(){}
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, true);
                    return Ok();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> POSTLogin([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    return result;
                }
            }
            return Microsoft.AspNetCore.Identity.SignInResult.Failed;
        }

        [HttpGet("Profile")]
        public async Task<UserProfileModelView?> GETProfile(string? returnUrl)
        {
            
            if (User == null || User.Identities == null || User.Identities.Count() > 0) 
                return null;
            if (!User.Identities.FirstOrDefault().IsAuthenticated)
                return null;
            string? Email = User.Identities?.FirstOrDefault()?.Name;
            if (string.IsNullOrEmpty(Email))
                return null;
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
                return new UserProfileModelView() {PhoneNumber = user?.PhoneNumber, Email = user?.Email };

            return null;
        }

        [HttpPost("Logout")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> POSTLogout(string? returnUrl)
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
