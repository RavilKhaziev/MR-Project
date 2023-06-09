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

        [HttpGet]
        [Route("Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<ProductInfoModel>> GetProducts()
        {
            List<ProductInfoModel> listProducts = new();
            foreach (var item in _db.ShopInfo.ToList())
            {
                listProducts.AddRange(item.Products.ConvertAll(ProductInfo.ToProductInfoModel));
            }
            

            return  listProducts;
        }
        [HttpGet]
        [Route("Products/{ShopName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<ProductInfoModel>> GetProducts(string ShopName)
        {
            List<ProductInfoModel> listProducts = new();
            foreach (var item in _db.ShopInfo.ToList())
            {
                listProducts.AddRange(item.Products.ConvertAll(ProductInfo.ToProductInfoModel));
            }

            return _db.ShopInfo.Where(p => p.Shop_Name == ShopName).FirstOrDefault().Products.ConvertAll(ProductInfo.ToProductInfoModel);
        }
    }
}
