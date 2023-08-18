using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagSetFavoriteViewModel
    {
        [Required]
        public Guid BagId { get; set; }

        [Required]
        public bool IsFavorite { get; set; }
    }
}
