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

            public Guid? PreviewImageId { get; set; }

            public uint Count { get; set; }

            public double Cost { get; set; }

            public List<string> Tags { get; set; } = new();

            public float? AvgEvaluation { get; set; }

            public List<Product.ProductViewModel> Products { get;set; } = new();

        }

        public List<Bag?> Bags { get; set; } = null!;
    }
}
