/// <summary>
/// Data Transfer Objects for Product entity.
/// Includes DTOs for reading, creating, updating and patching products.
/// </summary>

namespace SharedApp.Dto
{
    public record ProductReadDto
    (
        int ProductId, 
        string Name, 
        decimal Price,
        int Stock,
        string Category,
        bool Available,
        int SupplierId,
        string SupplierName,
        string SupplierLocation
    );

    public record ProductCreateDto
    (
        string Name,
        decimal Price,
        int Stock,
        string Category,
        int SupplierId
    );

    public record ProductUpdateDto
    (
        string Name,
        decimal Price,
        int Stock,
        string Category,
        int SupplierId
    );

    public record ProductPatchDto
    (
        string? Name = null,
        decimal? Price = null,
        int? Stock = null,
        string? Category = null,
        int? SupplierId = null
    );
}