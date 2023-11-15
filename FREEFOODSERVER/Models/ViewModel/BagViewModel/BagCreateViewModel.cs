using System.ComponentModel.DataAnnotations;
using FREEFOODSERVER.Models;


namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagCreateViewModel
    {
        public bool? IsDisabled { get; set; } = true;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        /// <summary>
        /// Первое изображение в списке - превью
        /// </summary>
        public List<string>? Images { get; set; }

        [Required]
        public uint Count { get; set; } = 0;

        [Required]
        public double Cost { get; set; } = 0;

        public List<string>? Tags { get; set;}

        public DateTime? Created { get; set; }

        public List<Product.ProductCreateViewModel>? Products { get; set; }

        //public static implicit operator Bag(BagCreateViewModel model)
        //{
        //    return new Bag()
        //    {
        //        Name = model.Name,
        //        Description = model.Description,
        //        ImagesId = model.ImagesId,
        //        Count = model.Count,
        //        Cost = model.Cost,
        //        Tags = model.Tags ?? new()
        //    };
        //}
    }
}
