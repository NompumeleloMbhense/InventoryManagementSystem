using SharedApp.Dto;

/// <summary>
/// Service interface for managing suppliers, providing
/// CRUD operations and additional functionalies like pagination and searching.
/// </summary>

namespace ServerApp.Services
{
    
    public interface ISupplierService
    {
        Task<(IEnumerable<SupplierReadDto> Suppliers, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<SupplierWithProductsDto> GetByIdAsync(int id);
        Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto);
        Task<SupplierReadDto> UpdateAsync(int id, SupplierUpdateDto dto);
        Task<SupplierReadDto> PatchAsync(int id, SupplierPatchDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<SupplierReadDto>> GetRecentAsync(int count);
        Task<int> GetTotalCountAsync();
    }
}