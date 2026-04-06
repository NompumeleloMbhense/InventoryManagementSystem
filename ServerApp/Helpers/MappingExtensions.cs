using SharedApp.Dto;
using SharedApp.Models;


/// <summary>
/// Extension methods for mapping domain models to DTOs
/// Centralizes mapping logic for cleaner controllers and services
/// </summary>

namespace ServerApp.Helpers
{
    public static class MappingExtensions
    {
        //1. Product Mappings
        public static ProductReadDto ToReadDto(this Product p) => new()
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            Category = p.Category,
            Available = p.Available,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.Name ?? "N/A",
            SupplierLocation = p.Supplier?.Location ?? "N/A"
        };


        // 2. Supplier Mappings
        public static SupplierReadDto ToReadDto(this Supplier s) => new()
        {
            SupplierId = s.SupplierId,
            Name = s.Name,
            Location = s.Location,
            Email = s.Email
        };


        // 3. Detailed Supplier Mapping including products
        public static SupplierWithProductsDto ToDetailedDto(this Supplier s) => new()
        {
            SupplierId = s.SupplierId,
            Name = s.Name,
            Location = s.Location,
            Email = s.Email,
            Products = s.Products.Select(p => new ProductForSupplierDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                Category = p.Category,
                Available = p.Available
            }).ToList()
        };
    }
}