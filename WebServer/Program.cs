using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using IdentityServer4.Services;
using IdentityServer4.Models;
using IdentityServer4;
using WebServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders()
				.AddDefaultUI();


//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

//builder.Services.AddIdentityServer()
//.AddDeveloperSigningCredential()
//.AddInMemoryApiResources(Config.C())
//.AddInMemoryClients(Config.Clients());

//builder.Services
//		.AddIdentityServer(x => x.IssuerUri = "null")
//		.AddSigningCredential(Certificate.Get())
//		.AddAspNetIdentity<ApplicationUser>()
//		.AddConfigurationStore(builder =>
//			builder.UseSqlServer(connectionString, options =>
//				options.MigrationsAssembly(migrationsAssembly)))
//		.AddOperationalStore(builder =>
//			builder.UseSqlServer(connectionString, options =>
//				options.MigrationsAssembly(migrationsAssembly)))
//		.Services.AddTransient<IProfileService, ProfileService>();
var app = builder.Build();

//app.UseIdentityServer();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	//app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
