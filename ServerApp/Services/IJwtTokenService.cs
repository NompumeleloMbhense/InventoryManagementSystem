using ServerApp.Models;

namespace ServerApp.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}