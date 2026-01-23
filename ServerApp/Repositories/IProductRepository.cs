using SharedApp.Models;

/// <summary>
/// Repository interface for managing products.
/// Defines methods for CRUD operations and searching products
/// </summary>

namespace ServerApp.Repositories
{
    public interface IProductRepository
    {
        //Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> SupplierExistsAsync(int supplierId);
        Task<IEnumerable<Product>> SearchAsync(string? query, string? category);
    }
}