using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using UdemyRabbitMQ.WaterMarkWeb.BackgroundServices;
using UdemyRabbitMQ.WaterMarkWeb.Models;
using UdemyRabbitMQ.WaterMarkWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase(databaseName:"productDb");
});

builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new  Uri(builder.Configuration["ConnectionStrings:RabbitMQ"]),DispatchConsumersAsync=true });
builder.Services.AddSingleton<RabbitMqClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddHostedService<ImageWatermarkProcessBackgroundService>();
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 3 0 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
