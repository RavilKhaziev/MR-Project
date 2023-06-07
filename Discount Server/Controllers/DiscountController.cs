using Microsoft.AspNetCore.Mvc;
using Discount_Server.ViewModels;
using Discount_Server.Models;

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
            await foreach (var item in _db.Shops.AsAsyncEnumerable())
            {
                list.Add(item);
            }
            return list.ConvertAll(ShopInfo.ToShopInfoModel);
        }


        /// <summary> 
        /// Задаёт доступные магазины POST запросом 
        /// </summary>

        [HttpPost]
        [Route("Shops")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async void PostShops(List<ShopInfoModel> newShops)
        {

            List<ShopInfo> list = newShops.ConvertAll(ShopInfoModel.ToShopInfo);
            try
            {
                await _db.Shops.AddRangeAsync(list);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
            }

        }

        [HttpGet]
        [Route("Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<ProductInfoModel>> GetProducts()
        {
            return new List<ProductInfoModel>();
        }

        [HttpPost]
        [Route("Products")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async void PostProducts()
        {
            
        }

       


    }
}
