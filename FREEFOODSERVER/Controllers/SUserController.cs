using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using FREEFOODSERVER.Models.ViewModel.Company;
using FREEFOODSERVER.Models.ViewModel.NSUser;
using FREEFOODSERVER.Models.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Runtime.CompilerServices;
using System.Security.Claims;

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
        private readonly ILogger<SUserController> _logger;
        private readonly ApplicationDbContext _db;

        private Dictionary<string, Func<IQueryable<Bag>, IQueryable<Bag>>> _filters = new()
        {
            { "popular", (x) =>  {return x.OrderBy(p => p.NumberOfViews); } }
        }; 
        

        

        public SUserController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            ILogger<SUserController> logger,
            ApplicationDbContext db
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task<IActionResult> POSTRegistration([FromBody] UserRegistrationViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Ошибка в данных регистрации");
            User user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true,
                UserInfo = new StandardUserInfo() {
                    UserName = model.Name
                }
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var ruleReturn = await _userManager.AddToRoleAsync(user, UserRoles.User);
                if (!ruleReturn.Succeeded)
                {
                    foreach (var error in ruleReturn.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await _userManager.DeleteAsync(user);
                    return BadRequest("Ошибка при выдаче роли");
                }
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
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> POSTLogin([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                return Ok(result);
            }
            return BadRequest(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GETProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Такого пользователя не существует");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var info = (StandardUserInfo)user.UserInfo;
            return Ok(new UserProfileViewModel() { PhoneNumber = user.PhoneNumber, Email = user.Email, Name = info.UserName });
        }

        [HttpPost("Logout")]
        public async Task POSTLogout()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Редактирование профиля
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Изменёный профиль</returns>
        [HttpPost("Profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTEditProfile([FromBody] UserProfileEditViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var info = ((StandardUserInfo)user.UserInfo);
            if (!string.IsNullOrEmpty(model.UserName)) info.UserName = model.UserName;
            return Ok(new UserProfileViewModel()
            {
               Name = model.UserName,
               PhoneNumber = user.PhoneNumber,
               Email = user.Email,
               
            });
        }

        [HttpPost("Bag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTBags([FromBody]IndexPageViewModel model)
        {
            var result = new List<Bag>();
            if (string.IsNullOrEmpty(model.Filter))
            {
            }
            else
            {
                var filters = model.Filter.Split();
                var bags = _db.Bags.AsNoTracking().IgnoreAutoIncludes().Include(x => x.Owner);
                foreach (var a in filters)
                {
                    if (_filters.ContainsKey(a))
                    {
                        _filters[a](bags);
                    }
                }
                bags.Take(BagPageViewModel.PAGESIZE);
            }

            return Ok();

        }


    }
}
