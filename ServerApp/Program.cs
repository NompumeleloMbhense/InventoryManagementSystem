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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Single DB for both app data and users
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDBConnection")));

// Identity configuration for app users
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// JWT authentication configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // JWT as default scheme
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // JWT as default challenge scheme
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

builder.Services.AddAuthorization();


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

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// Seed initial data
SeedData.EnsurePopulated(app);

// Seed roles & users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeedData.SeedRolesAndUsersAsync(userManager, roleManager);
}

app.Run();

