using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using SharedApp.Models;

/// <summary>
/// Repository implementation for managing products.
/// Provides methods for CRUD operations and searching products.
/// </summary>

namespace ServerApp.Repositories
{

    public class ProductRepository : IProductRepository
    {

        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Product>> GetPaginatedAsync(int pageNumber, int pageSize)
        {

            return await _db.Products
                .Include(p => p.Supplier)
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Get total count of products for pagination
        public async Task<int> GetTotalCountAsync()
        {
            return await _db.Products.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetRecentAsync(int count)
        {
            return await _db.Products
                    .OrderByDescending(p => p.ProductId)
                    .Take(count)
                    .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products
                            .Include(p => p.Supplier)
                            .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is null)
                return;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SupplierExistsAsync(int supplierId)
        {
            return await _db.Suppliers.AnyAsync(s => s.SupplierId == supplierId);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string? query, string? category)
        {
            // Start with all products
            var products = _db.Products
                .Include(p => p.Supplier)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(query))
                products = products.Where(p => p.Name.Contains(query));

            if (!string.IsNullOrWhiteSpace(category))
                products = products.Where(p => p.Category == category);

            // Execute the query and return results
            return await products.ToListAsync();
        }

        public async Task<int> GetLowStockCountAsync()
            => await _db.Products.CountAsync(p => p.Stock <= 5);
            
        
    }
}