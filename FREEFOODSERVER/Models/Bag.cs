using FREEFOODSERVER.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models
{
    public class Bag
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public User? Owner { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public List<string>? ImagesId { get; set; }

        [Required]
        public uint Count { get; set; } = 0;

        [Required]
        public double Cost { get; set; } = 0;

        public UInt64 NumberOfViews { get; set; }

        public bool IsFavorite { get; set; }
    }
}
