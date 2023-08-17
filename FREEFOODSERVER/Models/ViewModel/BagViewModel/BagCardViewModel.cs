using FREEFOODSERVER.Models.ViewModel.Company;
using Microsoft.EntityFrameworkCore;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagCardViewModel
    {
        public Guid Id { get; set; }

        public CompanyPreviewViewModel Company { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? PreviewImageId { get; set; }

        public uint Count { get; set; }

        public double Cost { get; set; }
    }
}
