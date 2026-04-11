using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Validators;
using SharedApp.Models;
using SharedApp.Dto;
using ServerApp.Repositories;
using ServerApp.Services;

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
        private readonly IProductService _service;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;
        private readonly IValidator<ProductPatchDto> _patchValidator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService service,
            IValidator<ProductCreateDto> createValidator,
            IValidator<ProductUpdateDto> updateValidator,
            IValidator<ProductPatchDto> patchValidator,
            ILogger<ProductsController> logger
        )
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _patchValidator = patchValidator;
            _logger = logger;
        }

        // READ endpoints - Publicly accessible
        // GET: api/products
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Ensure valid pagination inputs
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { error = "PageNumber and PageSize must be greater than 0" });

            var (products, totalCount) =
                await _service.GetPaginatedAsync(pageNumber, pageSize);

            return Ok(new
            {
                Data = products,
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
           => Ok(await _service.GetByIdAsync(id));



        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Create Product
        // POST: api/products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            // 1. Explicitly call the validator (Easier to test!)
            var validationResult = await _createValidator.ValidateAsync(dto);

            // 2. If it's not valid, throw the exception ourselves
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(
               nameof(GetById),
               new { id = result.ProductId }, result);
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Full Update
        // PUT: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            // Validate the incoming DTO
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) 
                throw new ValidationException(validationResult.Errors);

            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        // WRITE endpoints - Require login (JWT)
        // PATCH: api/products/5
        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, ProductPatchDto dto)
        {
            var validationResult = await _patchValidator.ValidateAsync(dto);
            if (!validationResult.IsValid) 
                throw new ValidationException(validationResult.Errors);

            var result = await _service.PatchAsync(id, dto);
            return Ok(result);
        }

        // WRITE endpoints - Require login (JWT)
        // ADMIN ONLY: Delete
        // DELETE: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }


        // READ endpoints — Publicly accessible
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] string? category)
        {
            var results = await _service.SearchAsync(query, category);

            return Ok(results);
        }


        // GET: api/products/count
        [AllowAnonymous]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var totalCount = await _service.GetTotalCountAsync();
            return Ok(totalCount);
        }

        // GET: api/products/recent/5
        [AllowAnonymous]
        [HttpGet("recent/{count}")]
        public async Task<IActionResult> GetRecent(int count)
        {
            var products = await _service.GetRecentAsync(count);

            return Ok(products);
        }

        // GET: api/products/lowstockcount
        [AllowAnonymous]
        [HttpGet("lowstockcount")]
        public async Task<IActionResult> GetLowStockCount()
        {
            var count = await _service.GetLowStockCountAsync();
            return Ok(count);
        }
    }
}