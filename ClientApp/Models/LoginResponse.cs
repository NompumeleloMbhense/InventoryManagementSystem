using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}