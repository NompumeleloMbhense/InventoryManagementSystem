using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Validators;
using SharedApp.Models;
using SharedApp.Dto;
using ServerApp.Repositories;
using ServerApp.Mappings;

/// <summary>
/// Controller for managing products.
/// Provides endpoints for CRUD operations and searching products.
/// </summary>


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


            return Ok(new
            {
                Data = products.Select(p => p.ToReadDto()),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
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


            return Ok(product.ToReadDto());
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Create Product
        // POST: api/products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {

            var validationResult = _createValidator.Validate(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);


            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                Category = dto.Category,
                SupplierId = dto.SupplierId
            };

            await _repo.AddAsync(product);


            return CreatedAtAction(nameof(GetById),
            new { id = product.ProductId },
            product.ToReadDto());

        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Full Update
        // PUT: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
                return NotFound(new { error = "Product not found" });

            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            // Check if SupplierId exists before updating
            var supplierExists = await _repo.SupplierExistsAsync(dto.SupplierId);
            if (!supplierExists)
                return BadRequest(new { message = "Invalid SupplierId." });


            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Category = dto.Category;
            product.SupplierId = dto.SupplierId;

            await _repo.UpdateAsync(product);

            return Ok(product.ToReadDto());
        }

        // WRITE endpoints - Require login (JWT)
        // USER: Only allowed to update Stock
        // ADMIN: Allowed to update all fields
        // PATCH: api/products/5
        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, ProductPatchDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
                return NotFound(new { error = "Product not found" });

            var validationResult = _patchValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            // Check supplier validity
            if (dto.SupplierId is not null)
            {
                var supplierExists = await _repo.SupplierExistsAsync(dto.SupplierId.Value);
                if (!supplierExists)
                    return BadRequest(new { error = "Invalid SupplierId" });

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

            // if (dto.SupplierId is not null)
            //     product.SupplierId = dto.SupplierId.Value;

            await _repo.UpdateAsync(product);

            return Ok(product.ToReadDto());
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Delete
        // DELETE: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
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

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCount()
        {
            var count = await _repo.GetTotalCountAsync();
            return Ok(count);
        }

        // GET: api/products/recent/{count}
        [HttpGet("recent/{count}")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetRecent(int count)
        {
            var products = await _repo.GetRecentAsync(count);
            var dtoList = products.Select(p => new ProductReadDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price
            });
            return Ok(dtoList);
        }
    }
}