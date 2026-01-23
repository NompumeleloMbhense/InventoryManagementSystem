using System.Net.Http.Json;
using ClientApp.Models;

/// <summary>
/// Service for managing suppliers.
/// Provides methods for CRUD operations and searching suppliers.
/// </summary>

namespace ClientApp.Services
{
    public class SupplierService
    {
        private readonly HttpClient _http;

        public SupplierService(HttpClient http)
        {
            _http = http;
        }

        // Get paginated suppliers using PagedResponse<T>
        public async Task<PagedResponse<SupplierReadDto>?> GetPaginatedAsync(int pageNumber = 1, int pageSize = 5)
        {
            var response = await _http.GetFromJsonAsync<PagedResponse<SupplierReadDto>>(
                $"api/suppliers?pageNumber={pageNumber}&pageSize={pageSize}");

            return response ?? new PagedResponse<SupplierReadDto>
            {
                Data = new List<SupplierReadDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            };
        }


        // Get single supplier with its products
        public async Task<SupplierWithProductsDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<SupplierWithProductsDto>($"api/suppliers/{id}");
        }

        // Create a supplier
        public async Task<bool> CreateAsync(SupplierCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/suppliers", dto);
            return response.IsSuccessStatusCode;
        }

        // Update a supplier
        public async Task<bool> UpdateAsync(int id, SupplierUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/suppliers/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        // Delete a supplier
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/suppliers/{id}");
            return response.IsSuccessStatusCode;
        }

        // TO DO: Add search functionality for suppliers 
    }
}