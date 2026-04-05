using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Class to generate JWT token for authenticated users to access protected resources 
/// like to access product and supplier data 
/// The token includes user ID and roles as claims
/// </summary>

namespace ServerApp.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        public JwtTokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }



        public async Task<string> GenerateTokenAsync(AppUser user)
        {
            // 1. Get Settings with Fallbacks
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Fetch User Roles
            var roles = await _userManager.GetRolesAsync(user);

            // 3. Build Claims
            // We use ClaimTypes.Name for the FullName so Blazor's @context.User.Identity.Name works automatically
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? "User"), // This shows in the UI
                new Claim("uid", user.Id)
            };

            // Add Roles as claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 4. Expiration Logic
            var duration = Convert.ToDouble(jwtSettings["DurationInMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(duration);

            // 5. Create Token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}