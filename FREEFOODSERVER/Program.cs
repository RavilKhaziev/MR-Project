using FREEFOODSERVER.Areas.Identity;
using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Runtime.CompilerServices;

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


            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                
                
            })
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
                    Title = "Сервис по индетификации пользователей",
                    Description = "Сервис управляет личными данными пользователей и их доступом к другим сервисам"
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            

            builder.Services.AddCors();
            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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
                var logger = services.GetRequiredService<ILogger<Program>>();
                var db = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                int count = 0;
                while (!db.Database.CanConnect())
                { 
                    logger.LogError($"Can't connect to DB. {connectionString}. Wait 5 sec.");
                    Task.Delay(500);
                    if (count > 100)
                        throw new Exception("Не возможно подключиться к БД");
                }
                try
                {
                    db.Database.Migrate(); 
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
                Console.WriteLine(rolesManager.Roles.ToList().Count);
                var result = RoleInitializer.InitializeAsync(userManager, rolesManager);
                result.Wait();
                db.SaveChanges();
                Console.WriteLine(rolesManager.Roles.ToList().Count);
            }

            app.Run();
        }
    }
}