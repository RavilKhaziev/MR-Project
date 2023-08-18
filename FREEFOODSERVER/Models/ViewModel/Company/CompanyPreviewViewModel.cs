using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.Company
{
    public class CompanyPreviewViewModel
    {
        public string Id { get; set; } = null!;
        
        public string? ImagePreview { get; set; }

        [Required]
        public string Name { get; set; } = null!;

    }
}
