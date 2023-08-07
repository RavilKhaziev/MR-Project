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
//{
//	// Describe certificate
//	string subject = "CN=localhost";
//	var rsaKey = RSA.Create(2048);

//	// Create certificate request
//	var certificateRequest = new CertificateRequest(
//		subject,
//		rsaKey,
//		HashAlgorithmName.SHA256,
//		RSASignaturePadding.Pkcs1
//	);

//	certificateRequest.CertificateExtensions.Add(
//		new X509BasicConstraintsExtension(
//			certificateAuthority: false,
//			hasPathLengthConstraint: false,
//			pathLengthConstraint: 0,
//			critical: true
//		)
//	);

//	certificateRequest.CertificateExtensions.Add(
//		new X509KeyUsageExtension(
//			keyUsages:
//				X509KeyUsageFlags.DigitalSignature
//				| X509KeyUsageFlags.KeyEncipherment,
//			critical: false
//		)
//	);

//	certificateRequest.CertificateExtensions.Add(
//		new X509SubjectKeyIdentifierExtension(
//			key: certificateRequest.PublicKey,
//			critical: false
//		)
//	);

//	certificateRequest.CertificateExtensions.Add(
//		new X509Extension(
//			new AsnEncodedData(
//				"Subject Alternative Name",
//				new byte[] { 48, 11, 130, 9, 108, 111, 99, 97, 108, 104, 111, 115, 116 }
//			),
//			false
//		)
//	);

//	var expireAt = DateTimeOffset.Now.AddYears(1);

//	var certificate1 = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, expireAt);

//	var exportableCertificate = new X509Certificate2(
//		certificate1.Export(X509ContentType.Cert),
//		(string)null,
//		X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
//	).CopyWithPrivateKey(rsaKey);

//	var passwordForCertificateProtection = new SecureString();
//	foreach (var @char in "p@ssw0rd")
//	{
//		passwordForCertificateProtection.AppendChar(@char);
//	}

//	// Export certificate to a file.
//	File.WriteAllBytes(
//		"certificateForServerAuthorization.pfx",
//		exportableCertificate.Export(
//			X509ContentType.Pfx,
//			passwordForCertificateProtection
//		)
//	);
//}

//var builder1 = Host.CreateDefaultBuilder(args)
//			.ConfigureWebHostDefaults(webBuilder =>
//			{
//				webBuilder
//					.UseKestrel(options =>
//					{
//						options.Listen(System.Net.IPAddress.Loopback, 44321, listenOptions =>
//						{
//							var connectionOptions = new HttpsConnectionAdapterOptions();
//							connectionOptions.ServerCertificate = certificate;

//							listenOptions.UseHttps(connectionOptions);
//						});
//					});
//			});

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
