using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.Product
{
    public class ProductCreateViewModel
    {
        public Guid BagId { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        public List<string>? ProductCategories { get; set; }
    }
}
