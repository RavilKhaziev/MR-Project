using FREEFOODSERVER.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace FREEFOODSERVER.Models
{
    public class Bag
    {
        public bool IsDisabled { get; set; } = true;

        [Key]
        public Guid Id { get; set; }

        [Required]
        public User? Company { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public List<string>? ImagesId { get; set; }

        [Required]
        public uint Count { get; set; } = 0;

        [Required]
        public double Cost { get; set; } = 0;

        public UInt64 NumberOfViews { get; set; }

        public List<string> Tags { get; set; } = new();

        public DateTime Created { get; set; } = DateTime.Now;

        public List<UserFeedback> Feedback { get; set; } = new();

        public float? AvgEvaluation { get; set; }
    }
}
