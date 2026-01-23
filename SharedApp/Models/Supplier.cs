/// <summary>
/// Supplier model representing a supplier entity.
/// </summary>

namespace SharedApp.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
    }
}