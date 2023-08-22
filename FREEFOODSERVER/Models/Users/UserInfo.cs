using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.Users
{
    public class UserInfo
    {
        [Key]
        public Guid Id { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
