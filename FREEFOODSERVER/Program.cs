using FREEFOODSERVER.Areas.Identity;
using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Services;
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

            var connectionStringUsers = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            NpgsqlDataSourceBuilder npgBuilderUser = new NpgsqlDataSourceBuilder(connectionStringUsers);
            using var dataSourceUsers = npgBuilderUser.Build();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(dataSourceUsers, builder => builder.EnableRetryOnFailure());
            });
            // Add services to the container.

            var connectionStringImages = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ImagesConnection' not found.");
            NpgsqlDataSourceBuilder npgBuilderImages = new NpgsqlDataSourceBuilder(connectionStringImages);
            using var dataSourceImages = npgBuilderImages.Build();
            builder.Services.AddDbContext<ImageDbContext>(options =>
            {
                options.UseNpgsql(dataSourceImages, builder => builder.EnableRetryOnFailure());
            });
            
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddMemoryCache();
            

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

            builder.Services.AddTransient<ImageManager>();
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
                var db_user = services.GetRequiredService<ApplicationDbContext>();
                var db_image = services.GetRequiredService<ImageDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                {
                    int count = 0;
                    while (!db_user.Database.CanConnect())
                    {
                        logger.LogError($"Can't connect to DB. {connectionStringUsers}. Wait 5 sec.");
                        Task.Delay(500);
                        if (count > 100)
                            throw new Exception("Не возможно подключиться к БД");
                    }
                    try
                    {
                        db_user.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                    Console.WriteLine(rolesManager.Roles.ToList().Count);
                    var result = RoleInitializer.InitializeAsync(userManager, rolesManager);
                    result.Wait();
                    db_user.SaveChanges();
                    Console.WriteLine(rolesManager.Roles.ToList().Count);
                }

                // Images 
                {
                    int count = 0;
                    while (!db_image.Database.CanConnect())
                    {
                        logger.LogError($"Can't connect to DB. {connectionStringUsers}. Wait 5 sec.");
                        Task.Delay(500);
                        if (count > 100)
                            throw new Exception("Не возможно подключиться к БД");
                    }
                    try
                    {
                        db_image.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                    db_image.SaveChanges();
                }
            }

            app.Run();
        }
    }
}