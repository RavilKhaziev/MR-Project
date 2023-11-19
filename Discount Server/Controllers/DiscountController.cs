using Microsoft.AspNetCore.Mvc;
using Discount_Server.ViewModels;
using Discount_Server.Models;
using SQLitePCL;
using Microsoft.AspNetCore.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

namespace Discount_Server.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    [Produces("application/json")]
    public class DiscountController : Controller
    {
        ApplicationDataBaseContext _db;
        ILogger<DiscountController> _logger;
        const uint PAGE_SIZE = 20;

        IMemoryCache _memoryCache;

        public DiscountController(ApplicationDataBaseContext AppContext, ILogger<DiscountController> logger, IMemoryCache memoryCache)
        {
            _db = AppContext;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Запрос на получение всех доступных магазинов
        /// </summary>
        /// <returns> Список всех доступных магазинов</returns>
        [HttpGet]
        [Route("Shops")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<ShopInfoModel>> GetShops()
        {
            List<ShopInfo> list = new();
            await foreach (var item in _db.ShopInfo.AsAsyncEnumerable())
            {
                list.Add(item);
            }
            return list.ConvertAll(ShopInfo.ToShopInfoModel);
        }
        /// <summary> 
        /// Запрос на получение всех доступных категорий товаров
        /// </summary>
        /// <returns> Список всех доступных категорий товаров</returns>
        [HttpGet]
        [Route("Products/Types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public List<string> GetProductsTypes()
        {
            return Parser.GetProductsCategory();
        }
        /// <summary> 
        /// Запрос на получение продуктов из определённого магазина.
        /// </summary>
        /// <param name="ShopName"> Указывает на необходимый магазин.</param>
        /// <param name="Category"> Указывает на необходимую категорию продукта.</param>
        /// <param name="Page"> 
        ///     Указывает какую страницу необходимо получить.
        ///     Нумерация страниц начинается с 0.
        ///     Количество товаров на странице 20.
        /// </param>
        /// <returns> Список всех продуктов из указанного магазина и категории.</returns>
        [HttpGet]
        [Route("Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ProductsPageViewModel GetProducts(string? ShopName = null, string? Category = null, uint Page = 0)
        {
            ShopInfo? shop = null;
            if (ShopName != null)
            {
                shop = _db.ShopInfo.Include(p => p.Products).AsNoTracking()
                    .Where((shop) => shop.Shop_Name == ShopName).FirstOrDefault();
            }

            var productList = new List<ProductInfoModel>();

            if (shop == null)
            {
                productList = _db.ProductInfo.AsNoTracking().ToList().ConvertAll(ProductInfo.ToProductInfoModel);
            }
            else
            {
                productList = shop.Products?.ToList().ConvertAll(ProductInfo.ToProductInfoModel);
            }

            if (Category != null)
            {
                productList = productList?.Where((p) => p.Type == Category).ToList();
            }
            ProductsPageViewModel productPage = new ProductsPageViewModel();

            if (productList.Count - Page * PAGE_SIZE < 0)
                productPage.Products = new List<ProductInfoModel>();
            else if ((productList.Count - (Page) * PAGE_SIZE) / PAGE_SIZE > 0)
                productPage.Products = productList.GetRange((int)(PAGE_SIZE * Page), (int)(PAGE_SIZE));
            else if ((productList.Count - (Page) * PAGE_SIZE) / PAGE_SIZE == 0)
                productPage.Products = productList.GetRange((int)(PAGE_SIZE * Page), (int)(productList.Count % PAGE_SIZE));

            productPage.PageInfo.TotalItems = productList.Count;
            productPage.PageInfo.PageNumber = (int)Page;
            productPage.PageInfo.PageSize = (int)PAGE_SIZE;
            return productPage;
        }
    }
}
