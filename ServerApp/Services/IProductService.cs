using SharedApp.Models;
using SharedApp.Dto;

/// <summary>
/// Service interface for managing products, providing
/// CRUD operations and additional functionalies like pagination and searching.
/// </summary>

namespace ServerApp.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductReadDto> Products, int TotalCount)> GetPaginatedAsync(int pageNumber,
         int pageSize);
        Task<ProductReadDto> GetByIdAsync(int id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto dto, string performedBy);
        Task<ProductReadDto> UpdateAsync(int id, ProductUpdateDto dto, string performedBy);
        Task<ProductReadDto> PatchAsync(int id, ProductPatchDto dto, string performedBy);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductReadDto>> SearchAsync(string? query, string? category);
        Task<IEnumerable<ProductReadDto>> GetRecentAsync(int count);
        Task<int> GetTotalCountAsync();
        Task<int> GetLowStockCountAsync();
        Task<IEnumerable<StockTransactionReadDto>> GetProductHistoryAsync(int productId);
    }
}