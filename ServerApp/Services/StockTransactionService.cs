using ServerApp.Repositories;
using ServerApp.Helpers;
using SharedApp.Dto;
using SharedApp.Models;


/// <summary>
/// Service implementation for managing stock tansactions. This service
/// provides methods for recording new stock transactions and retrieving 
/// the transaction history for a specific product.
/// </summary>

namespace ServerApp.Services
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IStockTransactionRepository _repo;

        public StockTransactionService(IStockTransactionRepository repo)
        {
            _repo = repo;
        }

        
        // Records a new stock transaction for a product, including the quantity change,
        // action type (e.g "Stock In", "Stock Out") and the user who performed it
        public async Task RecordAsync(int productId, int quantityChange, string actionType, string performedBy)
        {
            if(quantityChange == 0 ){
                return;
            }

            var transaction = new StockTransaction
            {
                ProductId = productId,
                QuantityChange = quantityChange,
                ActionType = actionType,
                PerformedBy = performedBy,
                DateOccurred = DateTime.UtcNow
            };

            await _repo.AddAsync(transaction);
        }


        // Retrieves the stock transaction history for a specific product and maps it to DTOs
        public async Task<IEnumerable<StockTransactionReadDto>> GetHistoryByProductAsync(int productId)
        {
            var logs = await _repo.GetByProductIdAsync(productId);
            return logs.Select(t => t.ToReadDto());
        }

    }
}