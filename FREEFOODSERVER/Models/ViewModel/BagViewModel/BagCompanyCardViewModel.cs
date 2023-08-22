using FREEFOODSERVER.Models.ViewModel.Company;
using Microsoft.EntityFrameworkCore;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagCompanyCardViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? PreviewImageId { get; set; }

        public uint Count { get; set; }

        public double Cost { get; set; }

        public List<string>? Tags { get; set; }

        public float? AvgEvaluation { get; set; }

        public bool IsDisabled { get; set; }

        public static implicit operator BagCompanyCardViewModel(Bag model) =>
            new() {
                Name = model.Name,
                Cost = model.Cost,
                Count = model.Count,
                Id = model.Id,
                PreviewImageId = model.ImagesId?.FirstOrDefault(),
                Tags = model.Tags,
                AvgEvaluation = model.AvgEvaluation,
                IsDisabled = model.IsDisabled,
            };
    }
}
