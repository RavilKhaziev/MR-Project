namespace FREEFOODSERVER.Models.Users
{
    public class CompanyInfo : UserInfo
    { 
        public string CompanyName { get; set; } = null!;

        public string? Discription { get; set; } 

        public List<Bag>? Bags { get; set; } = new List<Bag>();

        public string? ImagePreview { get; set; } 

        public float? AvgEvaluation { get; set; }

    }
}
