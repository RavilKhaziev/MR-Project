using Discount_Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Discount_Server.Models
{
    [Owned]
    public class ProductInfo
    {
        [Key]
        public int ProductInfoId { get; set; }
        public string? Product_Name { get; set; } 
        public string? Description { get; set; } 
        public string? Url { get; set; } 
        public string? Image_Url { get; set; } 
        public long? Sale_Price { get; set; }
        public string? Type { get; set; }

        [NotMapped]
        public static Converter<ProductInfo, ProductInfoModel> ToProductInfoModel = 
            (obj) => new ProductInfoModel
            {
                Name = obj.Product_Name,
                Description = obj.Description,
                Url = obj.Url,
                Image_Url = obj.Image_Url,
                Sale_Price = obj.Sale_Price,
                Type = obj.Type 
            };
    }
}
