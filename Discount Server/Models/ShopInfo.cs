using Discount_Server.ViewModels;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Discount_Server.Models
{

    [Index("Shop_Name", IsUnique = true)]
    public class ShopInfo : IEquatable<ShopInfo>
    {
        public int ShopInfoId { get; set; }
        public int Shop_Code { get; set; }
        
        public string? Shop_Name { get; set; }
        public string? Url { get; set; }

        public List<ProductInfo>? Products { get; set; } = new List<ProductInfo?>();

        [NotMapped]
        public static Converter<ShopInfo, ShopInfoModel> ToShopInfoModel =
            (obj) => new ShopInfoModel()
            {
                Name = obj.Shop_Name,
                Url = obj.Url,
                Shop_Code = obj.Shop_Code
            };

        public bool Equals(ShopInfo? x, ShopInfo? y)
        {
            return (x.Url == y.Url) && (x.Shop_Name == y.Shop_Name);
        }

        public bool Equals(ShopInfo? other)
        {
            if (other == null) { return false; }
            return (this.Url == other.Url) && (this.Shop_Name == other.Shop_Name);
        }
    }
}
