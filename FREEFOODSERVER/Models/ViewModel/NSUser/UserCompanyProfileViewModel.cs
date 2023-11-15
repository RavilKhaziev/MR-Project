namespace FREEFOODSERVER.Models.ViewModel.NSUser
{
    public class UserCompanyProfileViewModel
    {
        /// <summary>
        /// ID компании User
        /// </summary>
        public string? Id { get; set; }

        public Guid? ImagePreview { get; set; }

        public string CompanyName { get; set; } = null!;

        public float? AvgEvaluation { get; set; } 
    }
}
