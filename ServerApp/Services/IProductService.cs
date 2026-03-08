using SharedApp.Models;
using SharedApp.Dto;

namespace ServerApp.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<Product> GetByIdAsync(int id);
        Task<Product> CreateAsync(ProductCreateDto dto);
        Task<Product> UpdateAsync(int id, ProductUpdateDto dto);
        Task<Product> PatchAsync(int id, ProductPatchDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string? query, string? category);
        Task<IEnumerable<Product>> GetRecentAsync(int count);
        Task<int> GetTotalCountAsync();
        Task<int> GetLowStockCountAsync();
    }
}