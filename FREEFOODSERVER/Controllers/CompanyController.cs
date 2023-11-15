using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using FREEFOODSERVER.Models.ViewModel.Company;
using FREEFOODSERVER.Models.ViewModel.Product;
using FREEFOODSERVER.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FREEFOODSERVER.Controllers
{
    [ApiController]
    [Route("api/Company")]
    [Authorize(Roles = UserRoles.Company)]
    public class CompanyController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        
        private readonly ApplicationDbContext _db;
        private readonly ImageManager _imageManager;

        private static List<string> _filters { get; set; } = Bag.BagTags.Union(Product.Category).ToList();
        public CompanyController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db,
            ImageManager imageManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = db;
            _imageManager = imageManager;
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
        /// <response code="200"></response>
        [HttpPost("Registration")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTRegistration([FromBody] CompanyRegistrationViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            Company user = new()
            {
                Email = model.Email,
                EmailConfirmed = true,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                CompanyName = model.Name,
                Bags = new(),
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
                await _signInManager.SignInAsync(user, true).ConfigureAwait(false);
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
        /// <response code="200"></response>
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
            if (user == null) 
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;
            await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
            return Microsoft.AspNetCore.Identity.SignInResult.Success;
        }

        /// <summary>
        ///    Выход из аккаунта.
        /// </summary>
        [HttpPost("Logout")]
        public async Task POSTLogout()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///    Создаёт корзину для компании.
        /// </summary>
        /// <param name="model"> 
        /// </param>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        /// <response code="200"></response>
        [HttpPut("Bag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PUTBagCreate([FromBody] BagCreateViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");

            // var user = (Company?)await _userManager.FindByEmailAsync(email);
            var user = await _db.CompanyInfos.IgnoreAutoIncludes()
                .Include(x => x.Bags).ThenInclude(x => x.Products)
                .Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null) return NotFound("User no exist");

            user.Bags.Add(new()
            {
                Cost = model.Cost,
                Company = user,
                Count = model.Count,
                Description = model.Description,
                Name = model.Name,
                NumberOfViews = 0,
                Tags = model.Tags ?? new(),
                IsDisabled = model.IsDisabled ?? true,
                Created = model.Created ?? DateTime.Now,
            });

            var bag = user.Bags.Last();

            if (model.Images != null && model.Images.Count >= 1)
            {
                var images = model.Images.ConvertAll(
                    x => new ImageManager.AddImage()
                    {
                        Name = string.Concat(user.Id.ToString(), "_", bag.Id.ToString(), "_", bag.Id.GetHashCode()),
                        Image = _imageManager.GetImageFromString(x)
                    });

                var imagePrew = await _imageManager.SaveImageAsync(images.First());

                bag.ImagePreview = imagePrew;

                if (images.Count >= 2)
                {
                    var imagesList = await _imageManager.SaveImageRangeAsync(images.Take(1..));
                    bag.ImagesId = imagesList;
                }
            }

            bag.Products = model.Products?.ConvertAll<Product>(x => new()
            {
                Categories = x.ProductCategories?.Intersect(Product.Category).ToList() ?? new(),
                Name = x.ProductName,
                Bag = user.Bags.Last(),
            }) ?? new();

            foreach (var item in bag.Products)
                if (item.Categories != null)
                    bag.Filters = bag.Filters.Union(item.Categories).ToList();

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        ///     Позволяет получить все корзины или 1 определённого бокса.
        /// </summary>
        /// <param name="bagId"> Id запрашиваемой корзины.
        ///      !Внимание!
        ///         Если в теле запроса передать Id бокса,
        ///         то будет возвращено более детальное описание бокса.
        ///         Если не передать то возвратит все боксы.
        ///      !Внимание!
        /// </param>
        /// <returns>
        /// </returns>
        /// <response code="400">
        /// Возникает при:
        ///     Неверный запрос.
        ///     Ошибка в теле запрос или cookie(Нужно перезайти в аккаунт).
        /// </response>
        /// <response code="404">If the item is null</response>
        /// <response code="200"></response>
        [HttpPost("GetBag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTBagInfo([FromBody] Guid? bagId)
        {
            //var email = User.FindFirst(ClaimTypes.Email)?.Value;
            //if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            //var user = (Company?)await _userManager.FindByEmailAsync(email);
            //if (user == null) return NotFound("User no exist");

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _db.CompanyInfos.IgnoreAutoIncludes().AsNoTracking()
                .Include(x => x.Bags).ThenInclude(x => x.Products)
                .Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null) return NotFound("User no exist");

            if (bagId == null)
                return Ok(user.Bags.ConvertAll(x => (BagCompanyCardViewModel)x));
            else
            {
                Bag? bag;
                if ((bag = user.Bags.Find(x => x.Id == bagId)) == null) return NotFound("Bag not found");
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
        // TO DO REMOVE IMAGES 
        public async Task<IActionResult> DELETEBag([FromQuery] Guid bagId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = (Company?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            var bag = user.Bags.Find(x => x.Id == bagId);
            if (bag == null) return NotFound("Bag no exist");
            if (user.Bags.Remove(bag))
            {
                await _userManager.UpdateAsync(user).ConfigureAwait(false);
                return Ok();
            }
            else
                return BadRequest();
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
        [HttpGet("Profile")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GETProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = (Company?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");

            return Ok(new CompanyProfileViewModel()
            {
                CompanyName = user.CompanyName,
                Discription = user.Discription,
                ImagePreview = user.ImagePreview,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvgEvaluation = user.AvgEvaluation
            });
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
        public async Task<IActionResult> POSTEditProfile([FromBody] CompanyProfileEditViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = (Company?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (!string.IsNullOrEmpty(model.ImagePreview))
            {
                Image image;
                try
                {
                    image = Image.Load(Convert.FromBase64String(model.ImagePreview));
                }
                catch (Exception)
                {
                    return BadRequest("Error Image");
                }

                if (user.ImagePreview != null) await _imageManager.RemoveImageAsync((Guid)user.ImagePreview);


                user.ImagePreview = await _imageManager.SaveImageAsync(new()
                {
                    Image = image,
                    Name = user.Id.ToString(),
                });
            }

            if (!string.IsNullOrEmpty(model.CompanyName)) user.CompanyName = model.CompanyName;
            if (!string.IsNullOrEmpty(model.Discription)) user.Discription = model.Discription;
            await _userManager.UpdateAsync(user).ConfigureAwait(false);
            return Ok(new CompanyProfileViewModel()
            {
                CompanyName = user.CompanyName,
                Discription = user.Discription,
                ImagePreview = user.ImagePreview,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
            });
        }

        /// <summary>
        /// Реадактирование бокса 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Bag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BagInfoViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTBagEdit([FromBody] BagEditViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = (Company?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            var bag = user.Bags.Find(x => x.Id == model.Id);
            if (bag == null) return NotFound("Bag not found");

            if (!string.IsNullOrEmpty(model.Description)) bag.Description = model.Description;
            if (!string.IsNullOrEmpty(model.Name)) bag.Name = model.Name;
            if (model.Count != null) bag.Count = (uint)model.Count;
            if (model.Cost != null) bag.Cost = (double)model.Cost;
            if (model.Images != null)
            {
                bag.ImagesId = await _imageManager.EditImageRangeAsync(model.Images.ConvertAll<ImageManager.EditImage>(
                    x => new()
                    {
                        Id = x.Id,
                        Image = x.Image,
                    }));
            }
            if (model.ImagePreview != null)
            {
                bag.ImagePreview = await _imageManager.EditImageAsync(new() { Id = model.ImagePreview.Id, Image = model.ImagePreview.Image });
            }
            if (model.Tags != null) bag.Tags = model.Tags;
            if (model.IsDisabled != null) bag.IsDisabled = (bool)model.IsDisabled;

            await _userManager.UpdateAsync(user).ConfigureAwait(false);
            return Ok((BagInfoViewModel)bag);
        }

        /// <summary>
        /// Все доступные категории
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("Bag/Products/Categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GETProductsCategories()
        {
            return Ok(Product.Category);
        }

        /// <summary>
        /// Редактирование боксов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Bag/Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTProductEdit([FromBody] ProductEditViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _db.CompanyInfos.IgnoreAutoIncludes()
                .Include(x => x.Bags).ThenInclude(x => x.Products)
                .Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null) return NotFound("User no exist");

            Bag? bag;
            if ((bag = user.Bags.Find(x => x.Id == model.BagId)) == null) return NotFound("Bag not found");

            //var bag = await _db.Bags.Include(x => x.Products).Where(x => x.Id == model.BagId).FirstOrDefaultAsync();
            //if (bag == null) return BadRequest("Bag not exist");

            var product = bag.Products.Find(x => x.Id == model.ProductId);
            if (product == null) return BadRequest("Product not exist");

            if (!string.IsNullOrEmpty(model.ProductName)) product.Name = model.ProductName;
            product.Categories = model.ProductCategories?.Intersect(Product.Category).ToList();
            if (product.Categories != null)
                bag.Filters = bag.Filters.Union(product.Categories).ToList();


            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Ok("Saved");
        }

        /// <summary>
        /// Добавление продуктов в бокс
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Bag/Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PUTProductCreate([FromBody] ProductCreateViewModel model)
        {
            var bag = await _db.Bags.Include(x => x.Products).Where(x => x.Id == model.BagId).FirstOrDefaultAsync();
            if (bag == null) return BadRequest("Bag not exist");

            IEnumerable<string>? cat = model.ProductCategories != null ? Product.Category.Intersect(model.ProductCategories) : null;

            bag.Products.Add(new Product()
            {
                Name = model.ProductName,
                Categories = cat?.ToList(),
                Bag = bag
            });
            if (cat != null)
                bag.Filters = bag.Filters.Union(cat).ToList();

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Удаление продуктов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Bag/Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DELETEProductRemove([FromBody] ProductDeleteViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = await _db.CompanyInfos.IgnoreAutoIncludes()
                .Include(x => x.Bags)
                .ThenInclude(x => x.Products)
                .Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null) return NotFound("User no exist");

            Bag? bag;
            if ((bag = user.Bags.Find(x => x.Id == model.BagId)) == null) return NotFound("Bag not found");

            var product = bag.Products.Find(x => x.Id == model.ProductId);
            if (product == null) return NotFound("Product not exist");

            bag.Products.Remove(product);
            // Пересчитываем все теги и категории продуктов
            var filter = bag.Tags.AsQueryable();
            foreach (var item in bag.Products)
            {
                filter = filter.Union(item.Categories ?? new());
            }
            bag.Filters = filter.ToList();

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }


        /// <summary>
        /// Все доступные тэги для бокса
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("Bag/Tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GETBabTags()
        {
            return Ok(Bag.BagTags);
        }


    }
}
