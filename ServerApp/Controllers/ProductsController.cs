using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Validators;
using SharedApp.Models;
using SharedApp.Dto;
using ServerApp.Repositories;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;
        private readonly IValidator<ProductPatchDto> _patchValidator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductRepository repo,
            IValidator<ProductCreateDto> createValidator,
            IValidator<ProductUpdateDto> updateValidator,
            IValidator<ProductPatchDto> patchValidator,
            ILogger<ProductsController> logger
        )
        {
            _repo = repo;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _patchValidator = patchValidator;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Ok("Products API is running...");
        }
        
        // READ endpoints - Publicly accessible
        // GET: api/products
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Ensure valid pagination inputs
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { error = "PageNumber and PageSize must be greater than 0" });

            var products = await _repo.GetPaginatedAsync(pageNumber, pageSize);
            var totalCount = await _repo.GetTotalCountAsync();

            var response = new
            {
                // Take each product and build a smaller, cleaned up object version to send to client
                Data = products.Select(p => new ProductReadDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Category = p.Category,
                    Available = p.Available,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier?.Name ?? string.Empty,
                    SupplierLocation = p.Supplier?.Location ?? string.Empty
                }),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                // Calculate total pages of products
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)

            };
            return Ok(response);
        }

        // READ endpoint - Publicly accessible
        // GET: api/products/5
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
                return NotFound(new { error = "Product not found" });

            var result = new ProductReadDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                Available = product.Available,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.Name ?? string.Empty, // Include Supplier name if available
                SupplierLocation = product.Supplier?.Location ?? string.Empty // Include Supplier location if available
            };

            return Ok(result);
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Create Product
        // POST: api/products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            _logger.LogInformation("POST /api/products - Creating new product {@Product}", dto);

            try
            {
                var validationResult = _createValidator.Validate(dto);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for product {@Product}. Errors: {@Errors}", dto, validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }
                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    Category = dto.Category,
                    SupplierId = dto.SupplierId
                };

                await _repo.AddAsync(product);

                _logger.LogInformation("Product created successfully with ID {ProductId}", product.ProductId);

                var result = new ProductReadDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock,
                    Category = product.Category,
                    Available = product.Available,
                    SupplierId = product.SupplierId
                };

                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, result);
            }
            catch (InvalidOperationException ex)
            {
                // SupplierId invalid or other repository-level validation
                _logger.LogError(ex, "Invalid SupplierId provided for product {@Product}", dto);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating product {@Product}", dto);
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Full Update
        // PUT: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var existingProduct = await _repo.GetByIdAsync(id);
            if (existingProduct is null)
                return NotFound(new { error = "Product not found" });

            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            // Check if SupplierId exists before updating
            var supplierExists = await _repo.SupplierExistsAsync(dto.SupplierId); // you need this method in your repository
            if (!supplierExists)
                return BadRequest(new { message = "Invalid SupplierId." });


            existingProduct.Name = dto.Name;
            existingProduct.Price = dto.Price;
            existingProduct.Stock = dto.Stock;
            existingProduct.Category = dto.Category;
            existingProduct.SupplierId = dto.SupplierId;

            await _repo.UpdateAsync(existingProduct);

            var result = new ProductReadDto
            {
                ProductId = existingProduct.ProductId,
                Name = existingProduct.Name,
                Price = existingProduct.Price,
                Stock = existingProduct.Stock,
                Category = existingProduct.Category,
                Available = existingProduct.Available,
                SupplierId = existingProduct.SupplierId
            };

            return Ok(result);
        }

        // WRITE endpoints - Require login (JWT)
        // USER: Only allowed to update Stock
        // ADMIN: Allowed to update all fields
        // PATCH: api/products/5
        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, ProductPatchDto dto)
        {
            var existingProduct = await _repo.GetByIdAsync(id);
            if (existingProduct is null)
                return NotFound(new { error = "Product not found" });

            var validationResult = _patchValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            // Check supplier validity
            if (dto.SupplierId is not null)
            {
                var supplierExists = await _repo.SupplierExistsAsync(dto.SupplierId.Value);
                if (!supplierExists)
                    return BadRequest(new { error = $"Supplier with ID {dto.SupplierId.Value} does not exist." });
                existingProduct.SupplierId = dto.SupplierId.Value;
            }

            if (dto.Name is not null)
                existingProduct.Name = dto.Name;
            if (dto.Price is not null)
                existingProduct.Price = dto.Price.Value;
            if (dto.Stock is not null)
                existingProduct.Stock = dto.Stock.Value;
            if (dto.Category is not null)
                existingProduct.Category = dto.Category;
            if (dto.SupplierId is not null)
                existingProduct.SupplierId = dto.SupplierId.Value;

            await _repo.UpdateAsync(existingProduct);

            var result = new ProductReadDto
            {
                ProductId = existingProduct.ProductId,
                Name = existingProduct.Name,
                Price = existingProduct.Price,
                Stock = existingProduct.Stock,
                Category = existingProduct.Category,
                SupplierId = existingProduct.SupplierId
            };

            return Ok(result);
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Delete
        // DELETE: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingProduct = await _repo.GetByIdAsync(id);
            if (existingProduct is null)
                return NotFound(new { error = "Product not found" });

            await _repo.DeleteAsync(id);
            return NoContent();
        }


        // READ endpoints — Publicly accessible
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] string? category)
        {
            var products = await _repo.SearchAsync(query, category);

            var result = products.Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Price,
                p.Stock,
                p.Category,
                p.Available,
                p.SupplierId,
                SupplierName = p.Supplier?.Name ?? string.Empty,
                SupplierLocation = p.Supplier?.Location ?? string.Empty
            });

            return Ok(result);
        }
    }
}