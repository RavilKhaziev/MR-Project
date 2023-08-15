using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.Users
{
    public class UserInfo
    {
        [Key]
        public Guid Id { get; set; }
    }
}
