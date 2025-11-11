using Microsoft.AspNetCore.Identity;

namespace ServerApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Role { get; set; }
    }
}