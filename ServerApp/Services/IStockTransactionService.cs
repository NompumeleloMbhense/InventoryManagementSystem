using SharedApp.Dto;

/// <summary>
/// Service interface for managing stock transactions. This service
/// provides methods for recording new stock transactions and retrieving
/// the transaction history for a specific product. 
/// </summary>
namespace ServerApp.Services
{
    public interface IStockTransactionService
    {
        Task RecordAsync(int productId, int quantityChange, string actionType, string performedBy);
        Task<IEnumerable<StockTransactionReadDto>> GetHistoryByProductAsync(int productId);
    }
}