using Microsoft.AspNetCore.Identity;

namespace FREEFOODSERVER.Models.Users
{
    public class Admin : IdentityUser
    {
        public int BannedCount { get; set; }
    }
}
