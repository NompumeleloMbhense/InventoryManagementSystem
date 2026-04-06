/// <summary>
/// Data Transfer Objects for Product entity.
/// Shared by both ServerApp and ClientApp
/// </summary>

namespace SharedApp.Dto
{
    // 1. Used for listing products and getting product details
    // (GET: api/products and GET: api/products/{id})
    public record ProductReadDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool Available { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string SupplierLocation { get; set; } = string.Empty;
    }

    // 2. Used for creating a new product (POST: api/products)
    public record ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public int SupplierId { get; set; }
    }

    // 3. Used for updating an exixting product
    public record ProductUpdateDto : ProductCreateDto { }

    // 4. Used for partial update (PATCH: api/products/{id})
    public record ProductPatchDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? Category { get; set; }
        public int? SupplierId { get; set; }
    }
}