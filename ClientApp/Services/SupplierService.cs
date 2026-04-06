using System.Net.Http.Json;
using SharedApp.Dto; 

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

        public async Task<PagedResponse<SupplierReadDto>> GetPaginatedAsync(
            int pageNumber = 1, 
            int pageSize = 5, 
            string? searchTerm = null)
        {
            var url = $"api/suppliers?pageNumber={pageNumber}&pageSize={pageSize}";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            }

            var response = await _http.GetFromJsonAsync<PagedResponse<SupplierReadDto>>(url);

            return response ?? new PagedResponse<SupplierReadDto>
            {
                Data = Enumerable.Empty<SupplierReadDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<SupplierWithProductsDto?> GetByIdAsync(int id)
            => await _http.GetFromJsonAsync<SupplierWithProductsDto>($"api/suppliers/{id}");

        public async Task<bool> CreateAsync(SupplierCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/suppliers", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, SupplierUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/suppliers/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/suppliers/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetTotalCountAsync()
            => await _http.GetFromJsonAsync<int>("api/suppliers/count");

        public async Task<List<SupplierReadDto>> GetRecentAsync(int count)
            => await _http.GetFromJsonAsync<List<SupplierReadDto>>($"api/suppliers/recent/{count}") 
               ?? new List<SupplierReadDto>();
    }
}