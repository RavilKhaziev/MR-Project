using Discount_Server.Models;
using Discount_Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Discount_Server.Services
{
    static class BaseAddresses
    {
        static public string ParserAddressShops { get; private set; } = "https://localhost:8080/Shops";
        static public string ParserAddressProducts { get; private set; } = "https://localhost:8080/Products";
    }

    class Parser
    {
        public Parser() { }


        /// <summary> 
        /// Функция запрашивает у парсера, все доступные магазины для парсера.
        /// </summary>
        /// <returns>Лист магазинов. Значение может быть null.</returns>
        public async Task<List<ShopInfoModel>?> GetShopList()
        {
            
        }

        /// <summary> 
        /// Функция запрашивает у парсера, продукты конкретного магазина.
        /// </summary>
        /// <param name="shop"> Лист продуктов будет из данного магазина </param>
        /// <returns>Лист продуктов из указанного магазина. Значение может быть null.</returns>
        public async Task<List<ProductInfoModel>?> GetProductList(ShopInfo shop)
        {
            
        }
    }


    public class ParserService : BackgroundService
    {
        ILogger _logger;
        public ParserService(ILogger<ParserService> logger) => _logger = logger;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
