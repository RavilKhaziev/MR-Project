using FREEFOODSERVER.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;


namespace FREEFOODSERVER.Data
{
    public class ImageDbContext : DbContext
    {
        public DbSet<Bitmap> Images { get; set; } 
        public ImageDbContext(DbContextOptions<ImageDbContext> options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
    }
}
