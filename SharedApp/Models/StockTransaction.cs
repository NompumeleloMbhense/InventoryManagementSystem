/// <summary>
/// Represents a stock transaction, which can either be an addition
/// or a removal of stock for a specific product. This model is used to
/// track changes in inventory levels and maintain a history of stock movements
/// </summary>

namespace SharedApp.Models
{
    public class StockTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int QuantityChange { get; set; } // Postive for additions, negative for removals
        public string ActionType { get; set; } = string.Empty;

        public string PerformedBy { get; set; } = string.Empty; // The username from the JWT
        public DateTime DateOccurred { get; set; } = DateTime.UtcNow;

    }
}