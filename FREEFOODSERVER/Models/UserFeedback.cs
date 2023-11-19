using FREEFOODSERVER.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace FREEFOODSERVER.Models
{
    public class UserFeedback
    {
        [Key]
        public Guid Id { get; set; }

        public User? UserOwner { get; set; }

        /// <summary>
        /// Бокс
        /// </summary>
        public Bag? FeedbackOwner { get; set; } 

        public DateTime Time { get; set; }

        public ushort Evaluation { get; set; }
    }
}
