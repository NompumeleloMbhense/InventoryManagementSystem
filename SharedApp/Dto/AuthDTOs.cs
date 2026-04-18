using System.ComponentModel.DataAnnotations;


/// <summary>
/// DTOs related to authentication 
/// Includes login and registration request/response models
/// </summary>

namespace SharedApp.Dto
{
    // DTO for login request
    public record LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    // DTO for registration request
    public record RegisterDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 50 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;
    }

    // Used when receiving the token from the API
    public record LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}