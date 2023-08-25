using FREEFOODSERVER.Models.ViewModel.Company;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagUserCardViewModel
    {
        public CompanyPreviewViewModel Company { get; set; } = null!;

        public class Bag
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = null!;

            public string? PreviewImageId { get; set; }

            public uint Count { get; set; }

            public double Cost { get; set; }

            public List<string> Tags { get; set; } = new();

            public float? AvgEvaluation { get; set; } = null;
        }

        public List<BagUserCardViewModel.Bag?> Bags { get; set; }
    }
}
