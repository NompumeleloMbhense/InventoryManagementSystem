
/// <summary>
/// Data Transfer Objects for Supplier entity.
/// Includes DTOs for reading, creating, updating and patching suppliers.
/// </summary>
namespace SharedApp.Dto
{
    // 1. Used for listing suppliers (GET: api/suppliers)
    public record SupplierReadDto
    {
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // 2. Used for detailed view of a supplier with its products (GET: api/suppliers/{id})
    public record SupplierWithProductsDto : SupplierReadDto
    {
        public List<ProductForSupplierDto> Products { get; set; } = new();
    }

    // 3. Focused product info for supplier details
    public record ProductForSupplierDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool Available { get; set; }
    }


    //4. Used for creating a new supplier (POST: api/suppliers)
    public record SupplierCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // 5. Used for updating an existing supplier (PUT: api/suppliers/{id})
    public record SupplierUpdateDto : SupplierCreateDto { }

    // 6.Used for partial update (PATCH: api/suppliers/{id})
     public record SupplierPatchDto
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Email { get; set; }
    }

}