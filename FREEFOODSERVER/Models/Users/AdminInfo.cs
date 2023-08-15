namespace FREEFOODSERVER.Models.Users
{
    public class AdminInfo : UserInfo
    {
        public int Banned_Count { get; set; } = 0;
    }
}
