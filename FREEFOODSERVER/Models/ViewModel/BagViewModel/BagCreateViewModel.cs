using System.ComponentModel.DataAnnotations;
using FREEFOODSERVER.Models;


namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagCreateViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        /// <summary>
        /// Первое изображение в списке - превью
        /// </summary>
        public List<string>? ImagesId { get; set; }

        [Required]
        public uint Count { get; set; } = 0;

        [Required]
        public double Cost { get; set; } = 0;

        public static implicit operator Bag(BagCreateViewModel model)
        {
            return new Bag()
            {
                Name = model.Name,
                Description = model.Description,
                ImagesId = model.ImagesId,
                Count = model.Count,
                Cost = model.Cost,
            };
        }
    }
}
