using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace FREEFOODSERVER
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            NpgsqlDataSourceBuilder npgBuilder = new NpgsqlDataSourceBuilder(connectionString);
            using var dataSource = npgBuilder.Build();
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
            {
                    options.UseNpgsql(dataSource, builder => builder.EnableRetryOnFailure());
            });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddMvc();
            builder.Services.AddRazorPages();
            builder.Services.AddAuthentication().AddJwtBearer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "—ервис по индетификации пользователей",
                    Description = "—ервис управл€ет личными данными пользователей и их доступом к другим сервисам"
                });

            });
            builder.Services.AddCors();
            var app = builder.Build();

            app.UseCors();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
               
            }
            else
            {
                
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //  app.UseHsts();
            }
            app.UseMigrationsEndPoint();
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/IdentityServer/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/IdentityServer/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            //app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {

                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    db.Database.Migrate();
                    Console.WriteLine(rolesManager.Roles.ToList().Count);
                    var result = RoleInitializer.InitializeAsync(userManager, rolesManager);
                    result.Wait();
                    db.SaveChanges();
                    Console.WriteLine(rolesManager.Roles.ToList().Count);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.Run();
        }
    }
}