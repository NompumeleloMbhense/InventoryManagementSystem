/// <summary>
/// Supplier model representing a supplier entity.
/// </summary>

namespace SharedApp.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string Email { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}