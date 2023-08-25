using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FREEFOODSERVER.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Bag> Bags { get; set; }
        public DbSet<User> UserInfos { get; set; }
        public DbSet<Admin> AdminInfos { get; set; }
        public DbSet<Company> CompanyInfos { get; set; }

        public DbSet<IdentityUser> identityUsers { get; set; }
        
        public DbSet<UserFeedback> UserFeedbacks { get; set; } 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Company>().Navigation(e => e.Bags).AutoInclude();
            builder.Entity<User>().Navigation(e => e.FavoriteBags).AutoInclude();

        }
    }
}