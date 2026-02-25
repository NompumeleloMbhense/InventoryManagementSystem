using SharedApp.Models;

/// <summary>
/// Repository interface for managing suppliers.
/// Defines methods for CRUD operations and searching suppliers
/// </summary>

namespace ServerApp.Repositories
{
    public interface ISupplierRepository
    {
        //Task<IEnumerable<Supplier>> GetAllAsync();

        Task<IEnumerable<Supplier>> GetPaginatedAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<int> GetTotalCountAsync(string? searchTerm = null);
        Task<Supplier?> GetByIdAsync(int id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Supplier>> GetRecentAsync(int count);
    }
}