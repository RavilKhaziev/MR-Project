using Microsoft.AspNetCore.Identity;

namespace FREEFOODSERVER.Models.Users
{
    public class User : IdentityUser
    {
        public UserInfo? UserInfo { get; set; } = null;
    }
}
