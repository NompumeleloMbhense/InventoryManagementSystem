using System.Net.Http.Json;
using ClientApp.Models;

/// <summary>
/// Service for managing products.
/// Provides methods for CRUD operations and searching products.
/// </summary>

namespace ClientApp.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        // Paginated fetch of products
        public async Task<PagedResponse<ProductReadDto>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _http.GetFromJsonAsync<PagedResponse<ProductReadDto>>(
                 $"api/products?pageNumber={pageNumber}&pageSize={pageSize}");

            return response ?? new PagedResponse<ProductReadDto>
            {
                Data = new List<ProductReadDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            };
        }

        // Get a single product
        public async Task<ProductReadDto?> GetByIdAsync(int id)
            => await _http.GetFromJsonAsync<ProductReadDto>($"api/products/{id}");

        // Create a product
        public async Task<bool> AddAsync(ProductCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/products", dto);
            return response.IsSuccessStatusCode;
        }

        // Update a product
        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/products/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        // Delete a product
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/products/{id}");
            return response.IsSuccessStatusCode;
        }

        // Search for a product
        public async Task<IEnumerable<ProductReadDto>> SearchAsync(string? query, string? category)
        {
            var response = await _http.GetFromJsonAsync<IEnumerable<ProductReadDto>>(
                $"api/products/search?query={query}&category={category}");
            return response ?? new List<ProductReadDto>();
        }
    }
}