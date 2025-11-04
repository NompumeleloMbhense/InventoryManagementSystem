using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class SupplierFormModel
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(100, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}