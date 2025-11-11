using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Serilog;
using ServerApp.Data;
using ServerApp.Repositories;
using SharedApp.Validators;
using SharedApp.Dto;
using SharedApp.Models;
using ServerApp.Models;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Single DB for both app data and users
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDBConnection")));

// Identity configuration for app users
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Repositories Services
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Fluent Validation Service
builder.Services.AddValidatorsFromAssemblyContaining<SharedApp.Validators.ProductValidator>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5065")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Serilog configuration for logging to console and file
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();


// Global error handler for bad requests 
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var error = ex.Message.Contains("Implicit body inferred")
            ? "Request body is required."
            : ex.Message;

        await context.Response.WriteAsJsonAsync(new { error });
    }

});

app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseCors();

app.MapControllers();

// Seed initial data
SeedData.EnsurePopulated(app);

app.Run();

