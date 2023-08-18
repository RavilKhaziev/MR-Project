using FREEFOODSERVER.Models.ViewModel.Company;
using Microsoft.EntityFrameworkCore;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagCardViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? PreviewImageId { get; set; }

        public uint Count { get; set; }

        public double Cost { get; set; }

        public static implicit operator BagCardViewModel(Bag model) =>
            new() {
                Name = model.Name,
                Cost = model.Cost,
                Count = model.Count,
                Id = model.Id,
                PreviewImageId = model.ImagesId?.FirstOrDefault()
            };
    }
}
