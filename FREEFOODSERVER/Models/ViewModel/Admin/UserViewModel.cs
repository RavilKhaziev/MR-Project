using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace FREEFOODSERVER.Models.ViewModel.Admin
{
    public class UserViewModel
    {
        public IdentityUser user { get; set; } = new();

        public IList<string>? roles { get; set; }

    }
}
