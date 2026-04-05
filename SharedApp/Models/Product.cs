using System.Text.Json.Serialization;

/// <summary>
/// Product model representing an item in inventory.
/// </summary>

namespace SharedApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public required string Category { get; set; }
        public int SupplierId { get; set; }
        
        [JsonIgnore]
        public Supplier? Supplier { get; set; }
        
        public bool Available => Stock > 0;

    }
}