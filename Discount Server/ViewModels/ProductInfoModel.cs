using System.ComponentModel.DataAnnotations;
using Discount_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount_Server.ViewModels
{
    public class ProductInfoModel
    {
        // Название продукта
        public string? Name { get; set; } = null!;

        // Его описание - может быть пустым
        public string? Description { get; set; } = null;

        // Ссылка на сам продукт
        public string? Url { get; set; } = null!;

        // Ссылка на изображение продукта
        public string? Image_Url { get; set; } = null!;

        // Цена продукта
        public long? Sale_Price { get; set; } = null!;

        public string? Type { get; set; }

        static public Converter<ProductInfoModel, ProductInfo> ToProductInfo { get; private set; } =
            (model) => new ProductInfo()
            {
                Product_Name = model.Name,
                Description = model.Description,
                Url = model.Url,
                Image_Url = model.Image_Url,
                Sale_Price = model.Sale_Price,
                Type = model.Type
            };
    }



}
