using FREEFOODSERVER.Models.Users;

namespace FREEFOODSERVER.Models.ViewModel.Admin
{
    public class UserViewModel
    {
        public FREEFOODSERVER.Models.Users.User user { get; set; } = new();

        public IList<string>? roles { get; set; }

    }
}
