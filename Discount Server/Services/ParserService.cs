using Discount_Server.Models;
using Discount_Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Writers;
using System.Diagnostics;

namespace Discount_Server.Services
{

    public class ParserService : IHostedService, IDisposable
    {
        readonly ILogger _logger;
        //ApplicationDataBaseContext _db;
        IServiceProvider _serviceProvider;
        Timer? _timer = null;
        ulong _executionCount = 0;
        double _updateFrequency = 10000;

        public ParserService(ILogger<ParserService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Парсер запущен в фоновом режиме.");


            var list = new Parser().GetShopList().ConvertAll(ShopInfoModel.ToShopInfo);

            using (var scope = _serviceProvider.CreateScope())
            {
                var db =
                    scope.ServiceProvider
                        .GetRequiredService<ApplicationDataBaseContext>();
                var dbList = db.ShopInfo.ToList();
                foreach (var item in list)
                {
                    if (!dbList.Contains<ShopInfo>(item))
                        await db.AddAsync(item);
                }
                await db.SaveChangesAsync();
            }

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_updateFrequency));

        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Начало работы парсера...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            var count = Interlocked.Increment(ref _executionCount);

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider
                        .GetRequiredService<ApplicationDataBaseContext>();
                await db.ProductInfo.ExecuteDeleteAsync();
                foreach (var item in db.ShopInfo.ToList())
                {
                    List<ProductInfo>? list;
                    try
                    {
                       list = new Parser().GetProductList(ShopInfo.ToShopInfoModel(item))?.ConvertAll(ProductInfoModel.ToProductInfo);
                    }
                    catch (ParserException err)
                    {
                        _logger.LogError($"Что-то пошло не так при парсе: \n{err.Message}");
                        return;
                    }
                    catch(Exception err)
                    {
                        _logger.LogError($"Ошибка в работе парсера необходимо исправить!: \n{err.Message}");
                        return;
                    }
                    if (list == null)
                    {
                        return;
                    }
                    item.Products = list;
                    await db.SaveChangesAsync();
                }
            }


            _logger.LogInformation(
                $"Парсер завершил работу за {stopwatch.ElapsedMilliseconds} мс.\n" +
                $"Парсер был запущен уже: {count} раз.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Парсинг в фоновом режиме был остановлен.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
