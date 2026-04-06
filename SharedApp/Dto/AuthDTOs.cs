/// <summary>
/// DTOs related to authentication 
/// Includes login and registration request/response models
/// </summary>

namespace SharedApp.Dto
{   
    // DTO for login request
    public record LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // DTO for registration request
    public record RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Used when receiving the token from the API
    public record LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}