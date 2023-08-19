using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagEditViewModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Первое изображение в списке - превью
        /// </summary>
        public List<string>? ImagesId { get; set; }

        public uint? Count { get; set; }

        public double? Cost { get; set; }

        public bool? IsFavorite { get; set; }
    }
}
