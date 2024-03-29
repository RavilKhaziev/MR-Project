using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Discount_Server;
using Discount_Server.Services;
using Discount_Server.Models;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


string? SQLLiteConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMvc();

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDataBaseContext>(options => options.UseSqlite(SQLLiteConnection));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen((p) => 
{

    p.SwaggerDoc("v1.3.1", new()
    {
        Version = "v1.3.1",
        Title = "CyberUpgrade API - *�������� ��������*",
        Description = "API ��� ������ � ��������� ���������",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    p.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

builder.Services.AddHostedService<ParserService>();

builder.Services.AddMemoryCache();

builder.Services.AddResponseCompression();

builder.Services.AddCors();

//builder.Services.AddHostedService<TimedHostedService>();

var app = builder.Build();

app.UseResponseCompression();   

app.UseSwagger();

app.UseCors(builder => builder.AllowAnyOrigin());

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1.3.1/swagger.json", "v1.3.1");
    options.RoutePrefix = string.Empty;
});

app.UseStaticFiles();

//app.MapGet("/", () => "Hello World!");

app.MapControllerRoute(
    name : "default",
    pattern: "{controller=Discount}/{action=Shops}/{id?}");




app.Run();










