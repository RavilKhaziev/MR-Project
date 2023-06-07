using Discount_Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Discount_Server.ViewModels
{
    public class ShopInfoModel
    {

        // Код магазина - случайное число
        public int Shop_Code { get; set; }

        // Имя магазина
        public string? Name { get; set; } = null!;

        // Ссылка на главную страницу магазина  
        public string Url { get; set; } = null!;

        static public Converter<ShopInfoModel, ShopInfo> ToShopInfo =
           (model) => new ShopInfo()
           {
               Name = model.Name,
               Shop_Code = model.Shop_Code,
               Url = model.Url,
           };
    }

    // Пример
    /*
     * {
     *      {"Shope_Code", "123"}, 
     *      {"Name", "Пример названия магазина"},
     *      {"Url", "https://localhost:8080"}
     * }
     * 
     */
}
