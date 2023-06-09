using Discount_Server.ViewModels;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Discount_Server.Models
{

    [Index("Shop_Name", IsUnique = true)]
    public class ShopInfo
    {
        public int ShopInfoId { get; set; }
        public int Shop_Code { get; set; }
        
        public string? Shop_Name { get; set; }
        public string? Url { get; set; }

        public List<ProductInfo?>? Products { get; set; } = new List<ProductInfo?>();

        [NotMapped]
        public static Converter<ShopInfo, ShopInfoModel> ToShopInfoModel =
            (obj) => new ShopInfoModel()
            {
                Name = obj.Shop_Name,
                Url = obj.Url,
                Shop_Code = obj.Shop_Code
            };
    }
}
