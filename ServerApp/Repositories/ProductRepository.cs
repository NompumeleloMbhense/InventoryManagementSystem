using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using SharedApp.Models;

namespace ServerApp.Repositories
{

    public class ProductRepository : IProductRepository
    {

        private readonly AppDbContext _db;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext db, ILogger<ProductRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetPaginatedAsync(int pageNumber, int pageSize)
        {

            _logger.LogInformation("Fetching products page {PageNumber} with size {PageSize}", pageNumber, pageSize);

            var products = await _db.Products
            .Include(p => p.Supplier)
            .OrderBy(p => p.ProductId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            _logger.LogInformation("Fetched {Count} products", products.Count());

            return products;
        }

        // Get total count of products for pagination
        public async Task<int> GetTotalCountAsync()
        {
            return await _db.Products.CountAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products
                            .Include(p => p.Supplier) // This is to include related Supplier data
                            .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Adding product {@Product}", product);

                if (product.SupplierId != 0)
                {
                    var supplier = await _db.Suppliers.FindAsync(product.SupplierId);
                    if (supplier is null)
                    {
                        _logger.LogWarning("Invalid SupplierId {SupplierId} when adding product {@Product}", product.SupplierId, product);
                        throw new InvalidOperationException("Invalid SupplierId.");
                    }
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Product added successfully with ID {ProductId}", product.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {@Product}", product);
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is not null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SupplierExistsAsync(int supplierId)
        {
            return await _db.Suppliers.AnyAsync(s => s.SupplierId == supplierId);
        }
    }
}