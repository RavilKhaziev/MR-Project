using Microsoft.AspNetCore.Identity;

namespace FREEFOODSERVER.Models.Users
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = null!;
       
    }
}
