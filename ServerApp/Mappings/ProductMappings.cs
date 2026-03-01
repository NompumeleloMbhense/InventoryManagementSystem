using SharedApp.Dto;
using SharedApp.Models;

namespace ServerApp.Mappings
{
    public static class ProductMappings
    {
        public static ProductReadDto ToReadDto(this Product product)
        {
            return new ProductReadDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                Available = product.Available,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.Name ?? string.Empty,
                SupplierLocation = product.Supplier?.Location ?? string.Empty
            };
        }
    }
}