using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    public class ProductFormModel
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Please select a supplier")]
        public int SupplierId { get; set; }
    
    
    }
}