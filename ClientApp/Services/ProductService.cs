using System.Net.Http.Json;
using SharedApp.Dto;

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

        public async Task<PagedResponse<ProductReadDto>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _http.GetFromJsonAsync<PagedResponse<ProductReadDto>>(
                 $"api/products?pageNumber={pageNumber}&pageSize={pageSize}");

            return response ?? new PagedResponse<ProductReadDto>
            {
                Data = Enumerable.Empty<ProductReadDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
            => await _http.GetFromJsonAsync<ProductReadDto>($"api/products/{id}");

        public async Task<bool> CreateAsync(ProductCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/products", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/products/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/products/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ProductReadDto>> SearchAsync(string? query, string? category)
        {
            var response = await _http.GetFromJsonAsync<IEnumerable<ProductReadDto>>(
                $"api/products/search?query={query}&category={category}");
            return response ?? Enumerable.Empty<ProductReadDto>();
        }

        public async Task<int> GetTotalCountAsync()
            => await _http.GetFromJsonAsync<int>("api/products/count");

        public async Task<List<ProductReadDto>> GetRecentAsync(int count)
            => await _http.GetFromJsonAsync<List<ProductReadDto>>($"api/products/recent/{count}") 
               ?? new List<ProductReadDto>();

        public async Task<int> GetLowStockCountAsync()
            => await _http.GetFromJsonAsync<int>("api/products/lowstockcount");

    }
}