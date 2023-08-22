namespace FREEFOODSERVER.Models.ViewModel.Company
{
    public class CompanyProfileViewModel
    {
        public string CompanyName { get; set; } = null!;

        public string? Discription { get; set; }

        public string? ImagePreview { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public float? AvgEvaluation { get; set; }
    }
}
