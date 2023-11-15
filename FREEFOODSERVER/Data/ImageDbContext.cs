using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;


namespace FREEFOODSERVER.Data
{
    public class ImageDbContext : DbContext
    {
        public DbSet<ImageDb> Images { get; set; } 

        public DbSet<ImageDb.ImageData> ImageDatas { get; set; }

        public ImageDbContext(DbContextOptions<ImageDbContext> options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ImageDb>().HasIndex(u => u.Name)
                .IsUnique();
        }
    }
}
