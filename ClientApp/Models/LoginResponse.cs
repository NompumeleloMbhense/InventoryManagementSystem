using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class LoginResponse
    {
        public string token { get; set; } = string.Empty;
    }
}