using Discount_Server.Models;
using Discount_Server.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Discount_Server
{
    public class ApplicationDataBaseContext : DbContext
    {
        public DbSet<ProductInfo> Products { get; set; } = null!;
        public DbSet<ShopInfo> Shops { get; set; } = null!;
        public ApplicationDataBaseContext(DbContextOptions<ApplicationDataBaseContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        ~ApplicationDataBaseContext()
        {
            this.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }


    }
}
