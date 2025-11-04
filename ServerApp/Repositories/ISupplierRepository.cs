using SharedApp.Models;

namespace ServerApp.Repositories
{
    public interface ISupplierRepository
    {
        //Task<IEnumerable<Supplier>> GetAllAsync();

        Task<IEnumerable<Supplier>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(int id);
    }
}