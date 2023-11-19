using Discount_Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Discount_Server.ViewModels
{
    /// <summary>
    /// Информация о магазине
    /// </summary>
    public class ShopInfoModel
    {
        /// <summary>
        /// Код магазина - случайное число
        /// </summary>
        public int Shop_Code { get; set; }

        /// <summary>
        /// Имя магазина
        /// </summary>
        public string? Name { get; set; } = null!;

        /// <summary>
        /// Ссылка на главную страницу магазина  
        /// </summary>
        public string? Url { get; set; } = null!;

        static public Converter<ShopInfoModel, ShopInfo> ToShopInfo =
           (model) => new ShopInfo()
           {
               Shop_Name = model.Name,
               Shop_Code = model.Shop_Code,
               Url = model.Url
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
