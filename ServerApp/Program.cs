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
using ServerApp.Services;


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

// Password policy configuration for identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
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

builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();


// Global error handler for bad requests and unhandled exceptions
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();

        var exceptionFeature = context.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

        var exception = exceptionFeature?.Error;

        if (exception == null)
            return;

        logger.LogError(exception, "Unhandled exception occurred.");

        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message
                });
                break;
            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message
                });
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "An unexpected server error occurred."
                });
                break;
        }
    });
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

