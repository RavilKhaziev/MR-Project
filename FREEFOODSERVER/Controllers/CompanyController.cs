using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using FREEFOODSERVER.Models.ViewModel.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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


        /// <summary>
        ///   Регистрация аккаунта
        /// </summary>
        /// <param name="model"> 
        /// {
        ///    string Name - Имя пользователя
        ///    string Email - Почта пользователя 
        ///    string? PhoneNumber - номер телефона 
        ///    string Password - пароль 
        ///    string PasswordConfirm - подтверждение пароля
        /// }
        /// </param>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        ///  /// <response code="200"></response>
        [HttpPost("Registration")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTRegistration([FromBody]CompanyRegistrationViewModel model)
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

        /// <summary>
        ///    Вход в аккаунт. 
        ///    Данные сохраняються в cookie.
        /// </summary>
        /// <param name="model"> 
        /// {
        ///     string Email - Почта для входа
        ///     string Password - Пароль для входа 
        /// }
        /// </param>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        ///  /// <response code="200"></response>
        [HttpPost("Login")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        ///    Выход из аккаунта.
        /// </summary>
        [HttpPost("Logout")]
        public async Task POSTLogout()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        ///    Создаёт корзину для компании.
        /// </summary>
        /// <param name="model"> 
        /// {
        ///     string Name - Название бокса 
        ///     string? Description - Описание бокса
        ///     uint Count - Кол-во боксов
        ///     double Cost - Цена бокса
        /// }
        /// </param>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        ///  /// <response code="200"></response>
        [HttpPut("Bag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PUTBagCreate([FromBody] BagCreateViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            ((CompanyInfo)user.UserInfo).Bags.Add(model);
            var result = await _userManager.UpdateAsync(user);
            return Ok();
        }

        /// <summary>
        ///     Позволяет получить все корзины или 1 определённого бокса.
        ///     !!!См. описание параметров!!!
        /// </summary>
        /// <param name="bagId"> Id запрашиваемой корзины.
        ///      !Внимание!
        ///         Если в теле запроса передать Id бокса,
        ///         то будет возвращено более детальное описание бокса. 
        ///      !Внимание!
        /// </param>
        /// <returns>
        /// 
        /// Краткая информация
        /// [
        ///     {
        ///        Guid Id - Id бокса
        ///        string Name - Название бокса 
        ///        string? PreviewImageId - изображение для предпросмотра
        ///        uint Count - Кол-во боксов 
        ///        double Cost - Цена бокса 
        ///     }, ...
        /// ]
        /// 
        /// Полная информация
        /// {
        ///     Guid Id - Id бокса
        ///     string Name  - Название бокса 
        ///     string? Description - Описание бокса
        ///     List<string>? ImagesId - изображения бокса
        ///     uint Count - Кол-во боксов 
        ///     double Cost - Цена бокса 
        ///     bool IsFavorite - Избранное. Если true то бокса в избранном
        ///     UInt64 NumberOfViews - Кол-во просмотров
        /// }
        /// </returns>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        ///  /// <response code="200"></response>
        [HttpGet("Bag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GETBagInfo([FromBody] Guid? bagId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var info = (CompanyInfo)user.UserInfo;
            
            if (bagId == null)
                return Ok(info.Bags.ConvertAll(x => (BagCardViewModel)x));
            else
            {
                var bag = info.Bags.Find(x => x.Id == bagId);
                if (bag == null) return NotFound("User no Init");
                return Ok(new List<BagInfoViewModel>() { bag });
            }
        }

        /// <summary>
        /// Удаляет бокс
        /// </summary>
        /// <param name="bagId"> Id удаляемого бокса</param>
        /// <returns></returns>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        /// <response code="200"></response>
        [HttpDelete("Bag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DELETEBag([FromBody] Guid bagId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var bag = ((CompanyInfo)user.UserInfo).Bags.Find(x => x.Id == bagId);
            if (bag == null) return NotFound("Bag no exist");
            if (((CompanyInfo)user.UserInfo).Bags.Remove(bag))
            {
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Возвращает все избранные боксы.
        /// </summary>
        /// <returns>
        /// [   
        ///     {
        ///         public Guid Id - Id бокса
        ///         string Name - Название бокса 
        ///         string? PreviewImageId - изображение для предпросмотра
        ///         uint Count - Кол-во боксов 
        ///         double Cost - Цена бокса 
        ///     },...
        /// ]
        /// </returns>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        /// <response code="200"></response>
        [HttpGet("Bag/Favorite")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GETBagFavorite()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var bag = ((CompanyInfo)user.UserInfo).Bags.FindAll(x => x.IsFavorite).FirstOrDefault();
            if (bag == null) return NotFound("Bag no exist");

            return Ok(new BagCardViewModel()
            {
                Cost = bag.Cost,
                Id = bag.Id,
                Count = bag.Count,
                Name = bag.Name,
                PreviewImageId = bag.ImagesId?.FirstOrDefault(),
            });
        }



        /// <summary>
        /// Устанавливает избранную бокса.
        /// </summary>
        /// <param name="model">
        /// {
        ///     Guid BagId - Id бокса
        ///     bool IsFavorite - если true, то бокс избранный, иначе нет.
        /// }
        /// </param>
        /// <returns></returns>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        /// <response code="200"></response>
        [HttpPost("Bag/Favorite")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTBagSetFavorite([FromBody] BagSetFavoriteViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var bag = ((CompanyInfo)user.UserInfo).Bags.Find(x => x.Id == model.BagId);
            if (bag == null) return NotFound("Bag no exist");

            bag.IsFavorite = model.IsFavorite;
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        /// <summary>
        /// Возвращает описание профиля компании.
        /// </summary>
        /// <returns>
        /// {
        ///     string CompanyName - Название компании
        ///     string? Discription - Описание компании
        ///     string? ImagePreview - Иконка
        /// } 
        /// </returns>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        /// <response code="200"></response>
        [HttpPost("Profile")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GETProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (user.UserInfo == null) return NotFound("User no Init");
            var info = ((CompanyInfo)user.UserInfo);
            return Ok(new CompanyProfileViewModel()
            {
                CompanyName = info.CompanyName,
                Discription = info.Discription,
                ImagePreview = info.ImagePreview
            }) ;
        }

    }
}
