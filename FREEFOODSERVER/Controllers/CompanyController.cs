using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using FREEFOODSERVER.Models.ViewModel.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FREEFOODSERVER.Controllers
{
    [ApiController]
    [Route("api/Company")]
    [Authorize(Roles = UserRoles.Company)]
    public class CompanyController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _db;
        public CompanyController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            ApplicationDbContext db
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task<IActionResult> POSTRegistration([FromBody] CompanyRegistrationViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            User user = new User
            {
                Email = model.Email,
                EmailConfirmed = true,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserInfo = new CompanyInfo(){ 
                    CompanyName = model.Name,
                    Bags = new()
                }
            };

            

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var ruleReturn = await _userManager.AddToRoleAsync(user, UserRoles.Company);
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
                return BadRequest(ModelState);
            }

        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> POSTLogin([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return Microsoft.AspNetCore.Identity.SignInResult.Failed;
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, false);
                return Microsoft.AspNetCore.Identity.SignInResult.Success;
                }
            return Microsoft.AspNetCore.Identity.SignInResult.Failed;
        }

        [HttpPost("Logout")]
        public async Task POSTLogout()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Создание нового Bag-a для зарегестрированного пользователя
        /// </summary>
        /// <returns></returns>


        [HttpPut("Bag")]
        public async Task<IActionResult> PUTBagCreate([FromBody] BagCreateViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("User no exist");
            if (user.UserInfo == null) return BadRequest("User no Init");
            ((CompanyInfo)user.UserInfo).Bags.Add(model);
            var result = await _userManager.UpdateAsync(user);
            return Ok();
        }

        /// <summary>
        /// Получение всех Bag или одного
        /// </summary>
        /// <param name="bagId"></param>
        [HttpGet("Bag")]
        public async Task<IActionResult> GETBagInfo([FromBody] Guid? bagId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest(new List<BagInfoViewModel>());
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest(new List<BagInfoViewModel>());
            if (user.UserInfo == null) return BadRequest(new List<BagInfoViewModel>());
            var info = (CompanyInfo)user.UserInfo;
            var result = new List<BagInfoViewModel>();
            if (bagId == null)
            {
                result = info.Bags.ConvertAll(x => (BagInfoViewModel)x);
            }
            else
            {
                var bag = info.Bags.Find(x => x.Id == bagId);
                if (bag == null) return BadRequest(new List<BagInfoViewModel>());
                result = new List<BagInfoViewModel>() { bag };
            }
            return Ok(result);
        }

        [HttpDelete("Bag")]
        public async Task<IActionResult> DELETEBag([FromBody] Guid bagId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("User no exist");
            if (user.UserInfo == null) return BadRequest("User no Init");
            var bag = ((CompanyInfo)user.UserInfo).Bags.Find(x => x.Id == bagId);
            if (bag == null) return BadRequest("Bag no exist");
            if (((CompanyInfo)user.UserInfo).Bags.Remove(bag))
            {
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            else
                return BadRequest();
        }
    }
}
