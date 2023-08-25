using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using FREEFOODSERVER.Models.ViewModel.Company;
using FREEFOODSERVER.Models.ViewModel.Feedback;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<SUserController> _logger;
        private readonly ApplicationDbContext _db;
        private Dictionary<string, Func<IQueryable<Bag>, IQueryable<Bag>>> _filter = new()
        {
            { "popular", (x) =>  {return x.OrderBy(p => p.NumberOfViews); } }
        };

        private string[] _tags = new[] 
        {
            "breakfast",
            "lunch",
            "dinner",
            "vegan",
            "vegetarian",
        };
 
        public SUserController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
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

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                Name = model.Name,
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

        /// <summary>
        /// Вход
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
            var user = (User?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            return Ok(new UserProfileViewModel() { 
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name 
            });
        }
        
        /// <summary>
        /// Вход
        /// </summary>
        /// <returns></returns>
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
            var user = (User?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");
            if (!string.IsNullOrEmpty(model.UserName)) user.UserName = model.UserName;
            return Ok(new UserProfileViewModel()
            {
               Name = model.UserName,
               PhoneNumber = user.PhoneNumber,
               Email = user.Email,
            });
        }

        /// <summary>
        /// Получение всех компаний в порядке возрастания рейтинга
        /// </summary>
        [HttpGet("Company")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GETCompany()
        {
            var result = await _db.CompanyInfos.AsNoTracking().ToListAsync();
            return Ok(result.ConvertAll(x => new UserCompanyProfileViewModel()
            {
                CompanyName = x.CompanyName,
                AvgEvaluation = x.AvgEvaluation,
                ImagePreview = x.ImagePreview,
                Id = x.Id,
            }));
        }

        //[HttpPost("Company/GetBag")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GETBagInformation([FromBody] Guid companyId)
        //{
            
        //}

        /// <summary>
        /// Все известные тэги для сервера
        /// </summary>
        /// <returns></returns>
        [HttpGet("Tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GETBagsTags()
        {
            return Ok(_tags);
        }
        /// <summary>
        /// (В работе)Позволяет получить Боксы с применением фильтров и пагинацией. 
        /// </summary>
        /// <param name="model">
        ///     Отсчёт страниц начинаеться с 0.
        ///     Фильтры доступны по ...
        /// </param>
        /// <returns></returns>
        //TODO Функция перегружена необходимо разгрузить
        [HttpPost("Bag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> POSTBags([FromBody]IndexPageViewModel model)
        {
            List<BagUserCardViewModel> result = new();
            var pageInfo = new PageInfoViewModel();
            if (string.IsNullOrEmpty(model.Filter))
            {
                var count = _db.Bags.Count();
                if (model.Page * PageInfoViewModel.PAGESIZE > count)
                {
                    pageInfo.TotalItems = count;
                    pageInfo.PageNumber = -1;
                    return Ok(new OutIndexPageViewModel<BagUserCardViewModel>()
                    {
                        PageInfo = pageInfo,
                        Items = new()
                    });
                }
                if (model.Page == null) model.Page = 0;
                


                var bags = _db.Bags.AsNoTracking().IgnoreAutoIncludes().Include(x => x.Company)
                    .Where(x => !x.IsDisabled).AsEnumerable()
                    .Take(new Range(
                        Math.Clamp((int)model.Page * PageInfoViewModel.PAGESIZE, 0, count),
                        Math.Clamp((int)(model.Page + 1) * PageInfoViewModel.PAGESIZE, 0, count))
                    ).ToList();
                if (bags == null) bags = new();

                foreach (var item in bags)
                {
                    BagUserCardViewModel? company = null;
                    if ((company = result.Find(x => x.Company.Id == item.Company.Id)) != null){
                        company.Bags.Add(new BagUserCardViewModel.Bag()
                        {
                            AvgEvaluation = item.AvgEvaluation,
                            Cost = item.Cost,
                            Count = item.Count,
                            Id = item.Id, 
                            Name = item.Name,
                            PreviewImageId = item?.ImagesId?.FirstOrDefault(),
                            Tags = item.Tags
                        });
                    }
                    else
                    {
                        result.Add(new BagUserCardViewModel()
                        {
                            Company = new CompanyPreviewViewModel()
                            {
                                Id = item.Company.Id,
                                CompanyName = item.Company.CompanyName,
                                ImagePreview = item.Company.ImagePreview
                            },
                            Bags = new()
                            {
                                new BagUserCardViewModel.Bag()
                                {
                                    Id = item.Id,
                                    AvgEvaluation = item.AvgEvaluation, 
                                    Cost = item.Cost,
                                    Count = item.Count,
                                    Name = item.Name,
                                    PreviewImageId = item?.ImagesId?.FirstOrDefault(),
                                    Tags = item.Tags
                                }
                            }
                            
                        });
                    }
                }

                pageInfo.TotalItems = bags.Count;
            }
            else
            {
                var filters = model.Filter.Split();
                var filteredBags = _db.Bags.AsNoTracking().IgnoreAutoIncludes().Include(x => x.Company)
                    .Where(x => !x.IsDisabled);
                foreach ( var filter in filters) 
                {
                    filteredBags = filteredBags.Where(x => x.Tags.Contains(filter));
                }
                var count = filteredBags.Count();
                pageInfo.TotalItems = count;

                var bags = filteredBags.AsEnumerable()
                    .Take(new Range(
                        Math.Clamp((int)model.Page * PageInfoViewModel.PAGESIZE, 0, count),
                        Math.Clamp((int)(model.Page + 1) * PageInfoViewModel.PAGESIZE, 0, count)));

                foreach (var item in bags)
                {
                    BagUserCardViewModel? company = null;
                    if ((company = result.Find(x => x.Company.Id == item.Company.Id)) != null)
                    {
                        company.Bags.Add(new BagUserCardViewModel.Bag()
                        {
                            AvgEvaluation = item.AvgEvaluation,
                            Cost = item.Cost,
                            Count = item.Count,
                            Id = item.Id,
                            Name = item.Name,
                            PreviewImageId = item?.ImagesId?.FirstOrDefault(),
                            Tags = item.Tags
                        });
                    }
                    else
                    {
                        result.Add(new BagUserCardViewModel()
                        {
                            Company = new CompanyPreviewViewModel()
                            {
                                Id = item.Company.Id,
                                CompanyName = item.Company.CompanyName,
                                ImagePreview = item.Company.ImagePreview
                            },
                            Bags = new()
                            {
                                new BagUserCardViewModel.Bag()
                                {
                                    Id = item.Id,
                                    AvgEvaluation = item.AvgEvaluation,
                                    Cost = item.Cost,
                                    Count = item.Count,
                                    Name = item.Name,
                                    PreviewImageId = item?.ImagesId?.FirstOrDefault(),
                                    Tags = item.Tags
                                }
                            }

                        });
                    }
                }
            }

            // TO DO Компания = [BAGS]
            return Ok(new OutIndexPageViewModel<BagUserCardViewModel>() 
            {
                PageInfo = pageInfo,
                Items = result
            });

        }

        /// <summary>
        /// Установка отзыва
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Bag/Feedback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PUTBagSetFeedback([FromBody]FeedbackCreateViewModel model )
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email Error");
            var user = (User?)await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User no exist");

            Bag? bag ;
            if ((bag = await _db.Bags.Include(x => x.Feedback).Include(x => x.Company).FirstOrDefaultAsync(x => x.Id == model.BagId)) == null) 
                return NotFound("Bag no Exist");
            if (bag == null) return BadRequest("Server Error");
            if (bag.IsDisabled) return NotFound("Bag no Exist");
            if (bag.Company == null) return BadRequest("Server Error");
            var company = (Company?)bag.Company;
            if (company == null) return BadRequest("Server Error");

            var feedback = bag.Feedback.Find(x => x.UserOwner.Id == user.Id);

            if (feedback == null)
            {
                bag.Feedback.Add(new()
                {
                    Evaluation = model.Evaluation,
                    Time = model.Created,
                    UserOwner = user,
                    FeedbackOwner = bag
                });
            }
            else
            {
                feedback.Evaluation = model.Evaluation;
            }

            bag.AvgEvaluation = (float)bag.Feedback.Sum(x => x.Evaluation) / bag.Feedback.Count;
            company.AvgEvaluation = (float)company.Bags.Sum(x => (float)x.AvgEvaluation) / (float)company.Bags.Count;
            await _userManager.UpdateAsync(bag.Company);
            return Ok();
        }



    }
}
