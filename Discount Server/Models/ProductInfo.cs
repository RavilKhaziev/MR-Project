using Discount_Server.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Discount_Server.Models
{

    

    public class ProductInfo
    {
        [Key]
        public int ProductId { get; set; }

        ShopInfo Shop { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public string? Url { get; set; } = null!;
        public string? Image_Url { get; set; } = null!;
        public long? Sale_Price { get; set; } = null!;

        public string? Type { get; set; }
    }
}
