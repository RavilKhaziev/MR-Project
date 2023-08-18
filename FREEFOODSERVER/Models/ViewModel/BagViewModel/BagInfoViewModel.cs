using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagInfoViewModel
    {
        public Guid Id { get; set; }

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

        public bool IsFavorite { get; set; }

        public UInt64 NumberOfViews { get; set; }

        public static implicit operator BagInfoViewModel(Bag model)
        {
            return new()
            {
                Id = model.Id,
                ImagesId = model.ImagesId,
                Count = model.Count,
                Cost = model.Cost,
                Description = model.Description,
                Name = model.Name,
                IsFavorite = model.IsFavorite,
                NumberOfViews = model.NumberOfViews,
            };
        }
    }
}
