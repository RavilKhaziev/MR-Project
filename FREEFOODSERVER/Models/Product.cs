using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        public Bag? Bag { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public List<string>? Categories { get; set; }

        public readonly static string[] Category =
        {
                "Meat",
                "Bread",
                "Milk",
                "Vegetables",
                "Fruit",
        };
    }



}
