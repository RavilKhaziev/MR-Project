using Discount_Server.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Discount_Server.Models
{
    public class ShopInfo
    {
        [Key]
        public int ShopId { get; set; }
        public int Shop_Code { get; set; }
        public string? Name { get; set; } = null!;
        public string Url { get; set; } = null!;

        public static Converter<ShopInfo, ShopInfoModel> ToShopInfoModel = 
            (obj) => new ShopInfoModel()
            {
                Name = obj.Name,
                Url = obj.Url,
                Shop_Code = obj.Shop_Code
            };
    }
}
