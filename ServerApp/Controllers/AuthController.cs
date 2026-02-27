using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServerApp.Models;
using ServerApp.Services;
using SharedApp.Dto;

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
        private readonly JwtTokenService _jwtService;


        public AuthController(UserManager<AppUser> userManager,
                                IConfiguration config,
                                JwtTokenService jwtService)
        {
            _userManager = userManager;
            _config = config;
            _jwtService = jwtService;
        }


        // Self-registration for users meaning anyone can call this 
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // First, I check if a user with the same email already
            // exists to prevent duplicate accounts
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { Errors = new[] { "This email is already registered" } });

            // If the email is unique, I create a new AppUser
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            // Try to create the user with the provided password
            var result = await _userManager.CreateAsync(user, model.Password);

            // Handle errors during user creation (like password not meeting requirements)
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e =>
                                        e.Code == "DuplicateUserName" ? "This email is already registered." : e.Description);


                return BadRequest(new { Errors = errors });
            }

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
            var token = await _jwtService.GenerateTokenAsync(user);
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
    }
}