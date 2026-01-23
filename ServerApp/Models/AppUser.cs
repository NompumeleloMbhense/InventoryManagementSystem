using Microsoft.AspNetCore.Identity;

/// <summary>
/// Application user model extending IdentityUser
/// </summary>

namespace ServerApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
    
}