using ServerApp.Repositories;
using SharedApp.Dto;
using SharedApp.Models;

namespace ServerApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
                throw new KeyNotFoundException("Product not found");

            return product;
        }

        public async Task<Product> CreateAsync(ProductCreateDto dto)
        {
            if (!await _repo.SupplierExistsAsync(dto.SupplierId))
                throw new InvalidOperationException("Invalid SupplierId.");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                Category = dto.Category,
                SupplierId = dto.SupplierId
            };

            await _repo.AddAsync(product);
            return product;
        }

        public async Task<Product> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await GetByIdAsync(id);

            if (!await _repo.SupplierExistsAsync(dto.SupplierId))
                throw new InvalidOperationException("Invalid SupplierId.");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Category = dto.Category;
            product.SupplierId = dto.SupplierId;

            await _repo.UpdateAsync(product);
            return product;
        }

        public async Task<Product> PatchAsync(int id, ProductPatchDto dto)
        {
            var product = await GetByIdAsync(id);

            if (dto.SupplierId is not null)
            {
                if (!await _repo.SupplierExistsAsync(dto.SupplierId.Value))
                    throw new InvalidOperationException("Invalid SupplierId.");

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
            return product;

        }


        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            await _repo.DeleteAsync(product.ProductId);
        }

        public async Task<(IEnumerable<Product>, int)> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var products = await _repo.GetPaginatedAsync(pageNumber, pageSize);
            var total = await _repo.GetTotalCountAsync();
            return (products, total);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string? query, string? category)
        => await _repo.SearchAsync(query, category);

        public async Task<IEnumerable<Product>> GetRecentAsync(int count)
         => await _repo.GetRecentAsync(count);

         public async Task<int> GetTotalCountAsync()
            => await _repo.GetTotalCountAsync();
        

        public async Task<int> GetLowStockCountAsync()
            => await _repo.GetLowStockCountAsync();
        

    }
}