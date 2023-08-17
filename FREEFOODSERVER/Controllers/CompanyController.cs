using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using FREEFOODSERVER.Models.ViewModel.Company;
using FREEFOODSERVER.Models.ViewModel.NSUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

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
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Name,
                    EmailConfirmed = true,
                    PhoneNumber = model.PhoneNumber,
                    UserInfo = new CompanyInfo() {}
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
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> POSTLogin([FromBody] LoginViewModel model)
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
        public async Task<IActionResult> PUTBagCreate([FromBody]BagCreateViewModel model)
        {   
            var email = User.Identities.FirstOrDefault().Name;
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
        /// <returns></returns>
        [HttpGet("Bag")]
        public async Task<List<BagInfoViewModel>?> GETBagInfo([FromBody]Guid? bagId)
        {
            var email = User.Identities.FirstOrDefault().Name;
            if (string.IsNullOrEmpty(email)) return null;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;
            if (user.UserInfo == null) return null;
            var info = (CompanyInfo)user.UserInfo;
            var result = new List<BagInfoViewModel>();
            if (bagId == null)
            {
                result = info.Bags.ConvertAll(x => (BagInfoViewModel)x); 
            }
            else
            {
                var bag = info.Bags.Find(x => x.Id == bagId);
                if (bag == null) return null;
                result = new List<BagInfoViewModel>() { bag };
            }
            return result;
        }

        [HttpDelete("Bag")]
        public async Task<IActionResult> DELETEBag([FromBody]Guid bagId)
        {
            var email = User.Identities.FirstOrDefault().Name;
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
