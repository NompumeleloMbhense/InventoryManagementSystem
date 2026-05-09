using SharedApp.Models;

///<summary>
/// Repository interface for managing stock transactions. This 
/// interface defines methods for adding new stock transactions
/// and retrieving transaction history for a specific product.
/// </summary>

namespace ServerApp.Repositories
{
    public interface IStockTransactionRepository
    {
        Task AddAsync(StockTransaction transaction);
        Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId);
    }
}