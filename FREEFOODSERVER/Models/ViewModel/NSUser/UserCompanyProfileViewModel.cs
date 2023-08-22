namespace FREEFOODSERVER.Models.ViewModel.NSUser
{
    public class UserCompanyProfileViewModel
    {
        public Guid Id { get; set; }

        public string? ImagePreview { get; set; }

        public string CompanyName { get; set; } = null!;

        public ushort AvgEvaluation { get; set; } 
    }
}
