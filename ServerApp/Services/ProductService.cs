using ServerApp.Repositories;
using ServerApp.Helpers;
using SharedApp.Dto;
using SharedApp.Models;

/// <summary>
/// Service implemation for managing products, providing
/// CRUD operations and additional functionalies like pagination and searching.
/// </summary>

namespace ServerApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        // --- Helper Method (Internal use only) ---
        // Retrieves the Product entity from the database by ID 
        // and throws a KeyNotFoundException if it does not exist
        private async Task<Product> GetProductEntityAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");
            return product;
        }

        // Gets a paginated list of products with total count for pagination data
        public async Task<(IEnumerable<ProductReadDto> Products, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var products = await _repo.GetPaginatedAsync(pageNumber, pageSize);
            var total = await _repo.GetTotalCountAsync();

            var dtos = products.Select(p => p.ToReadDto());
            return (dtos, total);
        }

        // Gets a single product by ID, including its supplier details
        public async Task<ProductReadDto> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return product.ToReadDto();
        }
        
        // Creates a new product from the provide DTO and 
        // reurns the created product as a DTO,
        // throws InvalidOperationException if the specified supplier does not exist
        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
        {
            if (!await _repo.SupplierExistsAsync(dto.SupplierId))
                throw new InvalidOperationException("Supplier does not exist");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                Category = dto.Category,
                SupplierId = dto.SupplierId
            };

            await _repo.AddAsync(product);

            var created = await GetProductEntityAsync(product.ProductId);
            return created.ToReadDto();
        }


        // Updates an existing product with provided DTO data and 
        // reurns the updated product as a DTO,
        // throws KeyNotFoundException if the product does not exist
        public async Task<ProductReadDto> UpdateAsync(int id, ProductUpdateDto dto)
        {
            
            var product = await GetProductEntityAsync(id);

            if (!await _repo.SupplierExistsAsync(dto.SupplierId))
                throw new InvalidOperationException("Invalid SupplierId.");

            // Update the Entity properties
            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Category = dto.Category;
            product.SupplierId = dto.SupplierId;

            await _repo.UpdateAsync(product);
            return product.ToReadDto();
        }


        // Partially updates an existing product with provided DTO data and 
        // reruns the updated product as a DTO
        public async Task<ProductReadDto> PatchAsync(int id, ProductPatchDto dto)
        {
            var product = await GetProductEntityAsync(id);

            if (dto.SupplierId.HasValue)
            {
                if (!await _repo.SupplierExistsAsync(dto.SupplierId.Value))
                    throw new InvalidOperationException("Supplier does not exist.");
                product.SupplierId = dto.SupplierId.Value;
            }

            if (dto.Name is not null)
                product.Name = dto.Name;

            if (dto.Price is not null)
                product.Price = dto.Price.Value;

            if (dto.Stock is not null)
                product.Stock = dto.Stock.Value;

            if (dto.Category is not null)
                product.Category = dto.Category;

            await _repo.UpdateAsync(product);
            return product.ToReadDto();
        }

        // Deletes a product by ID, throws 
        // KeyNotFoundException if the product does not exist
        public async Task DeleteAsync(int id)
        {
            await GetProductEntityAsync(id);
            await _repo.DeleteAsync(id);
        }

        // Searches products by name and/or category, returns matching 
        // products as DTOs 
        public async Task<IEnumerable<ProductReadDto>> SearchAsync(string? query, string? category)
        {
            var results = await _repo.SearchAsync(query, category);
            return results.Select(p => p.ToReadDto());
        }

        // Gets the most recently added products, limited by the specified count
        public async Task<IEnumerable<ProductReadDto>> GetRecentAsync(int count)
        {
            var results = await _repo.GetRecentAsync(count);
            return results.Select(p => p.ToReadDto());
        }

        // Gets the total count of products in the database 
        public async Task<int> GetTotalCountAsync()
           => await _repo.GetTotalCountAsync();

        // Gets the count of products that are low in stock
        public async Task<int> GetLowStockCountAsync()
            => await _repo.GetLowStockCountAsync();

    }
}