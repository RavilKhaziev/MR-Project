using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models
{
    public class ImageDb
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ImageData? Data { get; set; }

        public class ImageData
        {
            [Key]
            public Guid Id { get; set; }

            public string Img { get; set; } = null!;
        }
    }
}
