namespace FREEFOODSERVER.Models.Users
{
    public class CompanyInfo : UserInfo
    { 
        public string? Discription { get; set; }

        public List<Bag> Bags { get; set; } = new List<Bag>();

        public string? ImagePreview { get; set; } 

    }
}
