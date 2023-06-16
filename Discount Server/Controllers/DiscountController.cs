using Microsoft.AspNetCore.Mvc;
using Discount_Server.ViewModels;
using Discount_Server.Models;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;

namespace Discount_Server.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    [Produces("application/json")]
    public class DiscountController : Controller
    {
        ApplicationDataBaseContext _db;
        ILogger<DiscountController> _logger;

        public DiscountController(ApplicationDataBaseContext AppContext, ILogger<DiscountController> logger)
        {
            _db = AppContext;
            _logger = logger;
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
        /// Запрос на получение всех доступных продуктов.
        /// </summary>
        /// <returns> Список всех продуктов</returns>
        [Route("Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<List<ProductInfoModel>> GetProducts()
        {
            return  (await _db.ProductInfo.ToListAsync()).ConvertAll(ProductInfo.ToProductInfoModel);
        }

        /// <summary>
        /// Запрос на получение продуктов из определённого магазина
        /// </summary>
        /// <param name="ShopName"> Указывает на необходимый магазин</param>
        /// <returns> Список всех продуктов из указанного магазина</returns>
        [HttpGet]
        [Route("Products/{ShopName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<List<ProductInfoModel>> GetProducts(string ShopName)
        {
            var productList = _db.ShopInfo.Include(p => p.Products).AsNoTracking()
                .Where((shop) => shop.Shop_Name == ShopName).FirstOrDefault();
            if (productList == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new List<ProductInfoModel>();
            }
            else
            {
                return productList.Products.ToList().ConvertAll(ProductInfo.ToProductInfoModel);
            }
        }
    }
}
