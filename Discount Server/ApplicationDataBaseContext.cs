using Discount_Server.Models;
using Discount_Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace Discount_Server
{
    public class ApplicationDataBaseContext : DbContext
    {
        public DbSet<ShopInfo> ShopInfo { get; set; } = null!;
        public DbSet<ProductInfo> ProductInfo { get; set; } = null!;

        public ApplicationDataBaseContext(DbContextOptions<ApplicationDataBaseContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.None);
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.None);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            //modelBuilder.Entity<ShopInfo>().HasIndex(u => u.Shop_Name).IsUnique();
        }


    }
}
