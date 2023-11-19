using System.ComponentModel.DataAnnotations;
using Discount_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount_Server.ViewModels
{
    /// <summary>
    /// Информация о продукте
    /// </summary>
    public class ProductInfoModel
    {
        /// <summary>
        /// Название продукта
        /// </summary>
        public string? Name { get; set; } = null!;

        /// <summary>
        /// Его описание - может быть пустым
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        /// Ссылка на сам продукт
        /// </summary>
        public string? Url { get; set; } = null!;

        /// <summary>
        /// Ссылка на изображение продукта
        /// </summary>
        public string? Image_Url { get; set; } = null!;

        /// <summary>
        /// Цена продукта
        /// </summary>
        public long? Sale_Price { get; set; } = null!;

        /// <summary>
        /// Категория продукта
        /// </summary>
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
