using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Serilog;
using System.Text;
using ServerApp.Data;
using ServerApp.Repositories;
using SharedApp.Validators;
using SharedApp.Dto;
using SharedApp.Models;
using ServerApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Services;
using ServerApp.Middleware;


var builder = WebApplication.CreateBuilder(args);

// -- 1. LOGGING (Serilog) --
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// --- 2. CORE SERVICES ---
builder.Services.AddControllers();

// Database configuration 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDBConnection")));

// Identity configuration for app users
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// --- 3. AUTHENTICATION & JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"] ?? "A_Very_Long_Default_Secret_Key_For_Development_Only";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
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
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();


// --- 4. Dependency Injection (Repositories and Services) ---
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<JwtTokenService>();

// Fluent Validation Service
builder.Services.AddValidatorsFromAssemblyContaining<SharedApp.Validators.ProductValidator>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Password policy configuration for identity
// builder.Services.Configure<IdentityOptions>(options =>
// {
//     options.Password.RequireDigit = true;
//     options.Password.RequireLowercase = true;
//     options.Password.RequireUppercase = true;
//     options.Password.RequireNonAlphanumeric = true;
//     options.Password.RequiredLength = 8;
// });

var app = builder.Build();


// 5. GLOBAL EXCEPTION HANDLER
app.UseMiddleware<GlobalExceptionMiddleware>();

// 6. MIDDLEWARE PIPELINE
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 7. DATA SEEDING
SeedData.EnsurePopulated(app);

// Seed roles & users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Seed Business Data
    SeedData.EnsurePopulated(app); 

    // Seed Identity Data
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var config = services.GetRequiredService<IConfiguration>();
    await IdentitySeedData.SeedRolesAndUsersAsync(userManager, roleManager, config);
}

app.Run();

