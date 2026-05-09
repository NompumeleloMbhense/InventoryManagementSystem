using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using SharedApp.Models;

namespace ServerApp.Repositories
{
    public class StockTransactionRepository : IStockTransactionRepository
    {
        private readonly AppDbContext _db;

        public StockTransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(StockTransaction transaction)
        {
            _db.StockTransactions.Add(transaction);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId)
        {
            // Retrieve all stock transactions for the specified product, ordered by date
            return await _db.StockTransactions
                .AsNoTracking()
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.DateOccurred)
                .ToListAsync();
            
        }
    }
}