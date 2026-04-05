using ServerApp.Repositories;
using ServerApp.Helpers;
using SharedApp.Dto;
using SharedApp.Models;

/// <summary>
/// Service Layer for Supplier-related business logic
/// Handles operations such as validation, mapping and coordination
/// between the API controllers and the data repositories
/// </summary>

namespace ServerApp.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;

        public SupplierService(ISupplierRepository repo)
        {
            _repo = repo;
        }

        // --- Private Helper to get the Database Entity ---
        private async Task<Supplier> GetSupplierEntityAsync(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier == null)
                throw new KeyNotFoundException($"Supplier with ID {id} not found");
            return supplier;
        }


        // Gets a paginated list of suppliers with optional search term
        public async Task<(IEnumerable<SupplierReadDto> Suppliers, int TotalCount)> GetPaginatedAsync(int pageNumber,
         int pageSize, string? searchTerm)
        {
            var suppliers = await _repo.GetPaginatedAsync(pageNumber, pageSize, searchTerm);
            var total = await _repo.GetTotalCountAsync(searchTerm);

            var dtos = suppliers.Select(s => s.ToReadDto());
            return (dtos, total);
        }


        // Gets a single supplier by ID, including their products
        public async Task<SupplierWithProductsDto> GetByIdAsync(int id)
        {
            var supplier = await GetSupplierEntityAsync(id);
            
            return supplier.ToDetailedDto();
        }
        
        
        // Creates a new supplier from the provided DTO 
        // and returns the created supplier as a DTO
        public async Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                Location = dto.Location,
                Email = dto.Email
            };

            await _repo.AddAsync(supplier);
            return supplier.ToReadDto();
        }

        
        // Updates an existing supplier with provided DTO 
        // data and returns the updated supplier as a updated DTO
        public async Task<SupplierReadDto> UpdateAsync(int id, SupplierUpdateDto dto)
        {
            var supplier = await GetSupplierEntityAsync(id);

            supplier.Name = dto.Name;
            supplier.Location = dto.Location;
            supplier.Email = dto.Email;

            await _repo.UpdateAsync(supplier);
            return supplier.ToReadDto();
        }

        // Partially updates an existing supplier with provided DTO 
        // data and returns the updated supplier as an updated DTO
        public async Task<SupplierReadDto> PatchAsync(int id, SupplierPatchDto dto)
        {
            var supplier = await GetSupplierEntityAsync(id);

            if (dto.Name is not null) 
                supplier.Name = dto.Name;
            if (dto.Location is not null) 
                supplier.Location = dto.Location;
            if (dto.Email is not null) 
                supplier.Email = dto.Email;

            await _repo.UpdateAsync(supplier);
            return supplier.ToReadDto();
        }


        // Deletes a supplier by ID, throws if not found or could not be deleted
        public async Task DeleteAsync(int id)
        {
            
            bool deleted = await _repo.DeleteAsync(id);
            if (!deleted)
                throw new KeyNotFoundException("Supplier not found or could not be deleted.");
        }

        // Gets the most recently added supplier, limited by the specified count
        public async Task<IEnumerable<SupplierReadDto>> GetRecentAsync(int count)
        {
            var results = await _repo.GetRecentAsync(count);
            return results.Select(s => s.ToReadDto());
        }

        // Gets the total count of suppliers
        public async Task<int> GetTotalCountAsync()
        {
            return await _repo.GetTotalCountAsync();
        }

    }
}