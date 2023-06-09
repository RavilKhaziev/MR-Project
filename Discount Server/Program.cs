using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using NgrokAspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Discount_Server;
using Discount_Server.Services;
using Newtonsoft.Json;
using Discount_Server.Models;

var builder = WebApplication.CreateBuilder(args);


string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddMvc();

builder.Services.AddControllers();


builder.Services.AddDbContext<ApplicationDataBaseContext>(options => options.UseSqlite(connection));

builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<ParserService>();


//builder.Services.AddHostedService<TimedHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseStaticFiles();

//app.MapGet("/", () => "Hello World!");

app.MapControllerRoute(
    name : "default",
    pattern: "{controller=Discount}/{action=Shops}/{id?}");




app.Run();










