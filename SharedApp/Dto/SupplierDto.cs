/// <summary>
/// Data Transfer Objects for Supplier entity.
/// Includes DTOs for reading, creating, updating and patching suppliers.
/// </summary>

namespace SharedApp.Dto
{
    public record SupplierReadDto
    (
        // Used for listing suppliers (GET: api/suppliers)
        int SupplierId,
        string Name,
        string Location,
        string Email
    );

    // Used for detailed view of a supplier with its products (GET: api/suppliers/{id})
    public record SupplierWithProductsDto
    (
        int SupplierId,
        string Name,
        string Location,
        string Email,
        List<ProductForSupplierDto> Products): SupplierReadDto(SupplierId, Name, Location, Email
    );

    // Focused product info for supplier details
    public record ProductForSupplierDto
    (
        int ProductId,
        string Name,
        decimal Price,
        int Stock,
        string Category,
        bool Available
    );

    // Used for creating a new supplier (POST: api/suppliers)
    public record SupplierCreateDto
    (
        string Name,
        string Location,
        string Email
    );

    // Used for updating an existing supplier (PUT: api/suppliers/{id})
    public record SupplierUpdateDto
    (
        string Name,
        string Location,
        string Email
    );

    // Used for partial update (PATCH: api/suppliers/{id})
    public record SupplierPatchDto
    (
        string? Name = null,
        string? Location = null,
        string? Email = null
    );

}