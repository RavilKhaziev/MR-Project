using FREEFOODSERVER.Models;
using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FREEFOODSERVER.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Bag> Bags { get; set; }
        public DbSet<StandardUserInfo> UserInfos { get; set; }
        public DbSet<AdminInfo> AdminInfos { get; set; }
        public DbSet<CompanyInfo> CompanyInfos { get; set; }

        
        
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
            builder.Entity<User>().Navigation(e => e.UserInfo).AutoInclude();
            builder.Entity<CompanyInfo>().Navigation(e => e.Bags).AutoInclude();
        }
    }
}