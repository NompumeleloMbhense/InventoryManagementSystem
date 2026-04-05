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
        // Product Mappings
        public static ProductReadDto ToReadDto(this Product p) => new(
            p.ProductId,
            p.Name,
            p.Price,
            p.Stock,
            p.Category,
            p.Available,
            p.SupplierId,
            p.Supplier?.Name ?? "N/A",
            p.Supplier?.Location ?? "N/A"
        );

        // Supplier Mappings
        public static SupplierReadDto ToReadDto(this Supplier s) => new(
            s.SupplierId,
            s.Name,
            s.Location,
            s.Email
        );

        public static SupplierWithProductsDto ToDetailedDto(this Supplier s) => new(
            s.SupplierId,
            s.Name,
            s.Location,
            s.Email,
            s.Products.Select(p => new ProductForSupplierDto(
                p.ProductId,
                p.Name,
                p.Price,
                p.Stock,
                p.Category,
                p.Available
            )).ToList()
        );
    }
}