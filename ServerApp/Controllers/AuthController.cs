using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Controller responsible for user authentication
/// Handles user registration and Login
/// </summary>


namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }


        // Self-registration for users meaning anyone can call this 
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            // Create the user
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assigning "User" role to the newly registered user
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { Message = "User registered successfully" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            // First, I am trying to find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            // If the user does not exist or password is incorrect, return Unauthorized response
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { Message = "Invalid email or password" });

            // After verifying user credential, I am generating a JWT token
            // So that the client (the frontend) can use it for requesting protected resources
            var token = await GenerateJwtToken(user);
            return Ok(new { token }); // Returning this token to the frontend to store and use for subsequent requests
        }

         // Admin-only endpoint to promote a user to Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("promote/{userId}")]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest(new { Message = "User is already an Admin" });

            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok(new { Message = "User promoted to Admin successfully" });
        }

         // Logout endpoint (client should delete JWT, optional server-side invalidation)
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In JWT, logout is typically handled on the client by discarding the token
            return Ok(new { Message = "Logout successful. Please discard your token client-side." });
        }

        // Method to generate JWT token for authenticated users to access protected resources 
        // like to access product and supplier data 
        // The token includes user ID and roles as claims
        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            // Getting user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Defining token claims
            // Creating a JWT token for a user who has successfully logged in
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("uid", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            // Adding role claims to the token
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Creating the token to be signed for security
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs for registration and Login
    // I chose record types for simplicity
    public record RegisterDto(string FullName, string Email, string Password);
    public record LoginDto(string Email, string Password);
}