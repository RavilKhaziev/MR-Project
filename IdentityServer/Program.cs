using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using IdentityServer4.Services;
using IdentityServer4.Models;
using IdentityServer4;
using WebServer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Text;
using Npgsql;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using IdentityServer.Models.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using IdentityServer.Models.Roles;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityServer;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDataProtection();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

NpgsqlDataSourceBuilder npgBuilder = new NpgsqlDataSourceBuilder(connectionString);
await using var dataSource = npgBuilder.Build();

builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseNpgsql(dataSource, builder => builder.EnableRetryOnFailure())
	 .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
	
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
		.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();



builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
	options =>
	{
		options.SaveToken = true;
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuer = true,
			// строка, представляющая издателя
			ValidIssuer = AuthOptions.ISSUER,

			// будет ли валидироваться потребитель токена
			ValidateAudience = false,
			// установка потребителя токена
			ValidAudience = AuthOptions.AUDIENCE,
			// будет ли валидироваться время существования
			ValidateLifetime = true,

			// установка ключа безопасности
			IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
			// валидация ключа безопасности
			ValidateIssuerSigningKey = false,
		};
	}).AddCookie();
//	}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
//var multiSchemePolicy = new AuthorizationPolicyBuilder(
//    CookieAuthenticationDefaults.AuthenticationScheme,
//    JwtBearerDefaults.AuthenticationScheme)
//  .RequireAuthenticatedUser()
//  .Build();





//builder.Services.AddIdentityServer(options =>
//{
//	options.IssuerUri = "null";
//	options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

//	options.Events.RaiseErrorEvents = true;
//	options.Events.RaiseInformationEvents = true;
//	options.Events.RaiseFailureEvents = true;
//	options.Events.RaiseSuccessEvents = true;
//})
//.AddInMemoryIdentityResources(Config.GetResources())
//.AddInMemoryApiScopes(Config.GetApiScopes())
//.AddInMemoryApiResources(Config.GetApis())
//.AddInMemoryClients(Config.GetClients(builder.Configuration))
//.AddAspNetIdentity<AppUser>()
//.AddDeveloperSigningCredential();


builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Сервис по индетификации пользователей",
		Description = "Сервис управляет личными данными пользователей и их доступом к другим сервисам"
	});

});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;

    options.LoginPath = "/api/Admin/Login";
    options.SlidingExpiration = true;
});

builder.Services.AddMvc();

var app = builder.Build();

app.UseCors(x => x// путь к нашему SPA клиенту
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

//app.UseSession();

using (var scope = app.Services.CreateScope())
{
	
	var services = scope.ServiceProvider;
	try
	{
		var db = services.GetRequiredService<ApplicationDbContext>();
		var userManager = services.GetRequiredService<UserManager<AppUser>>();
		var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		await RoleInitializer.InitializeAsync(userManager, rolesManager);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
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
}
else
{
	//app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	//app.UseHsts();
}

app.UseRouting();
//app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseIdentityServer();
app.UseAuthentication();

app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//	endpoints.MapControllers();
//});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	var context = services.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();
	//context.Database.EnsureCreated();
	// DbInitializer.Initialize(context);
}


app.Run();
