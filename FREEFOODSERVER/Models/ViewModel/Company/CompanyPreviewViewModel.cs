using FREEFOODSERVER.Models.ViewModel.BagViewModel;
using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.Company
{
    public class CompanyPreviewViewModel
    {
        public string Id { get; set; }
        
        public Guid? ImagePreview { get; set; }

        public string CompanyName { get; set; } = null!;

        
    }
}
