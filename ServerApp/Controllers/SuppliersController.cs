using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServerApp.Services;
using SharedApp.Dto;

/// <summary>
/// Controller for managing suppliers.
/// Provides endpoints for CRUD operations and searching suppliers.
/// </summary>

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;
        private readonly IValidator<SupplierCreateDto> _createValidator;
        private readonly IValidator<SupplierUpdateDto> _updateValidator;
        private readonly IValidator<SupplierPatchDto> _patchValidator;

        public SuppliersController(
                  ISupplierService service,
                  IValidator<SupplierCreateDto> createValidator,
                  IValidator<SupplierUpdateDto> updateValidator,
                  IValidator<SupplierPatchDto> patchValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _patchValidator = patchValidator;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var (suppliers, totalCount) = await _service.GetPaginatedAsync(pageNumber, pageSize, searchTerm);

            // FIXED: Use the actual DTO class instead of 'new { ... }'
            var response = new PagedResponse<SupplierReadDto>
            {
                Data = suppliers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }


        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
           => Ok(await _service.GetByIdAsync(id));



        // POST: api/suppliers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(SupplierCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.SupplierId }, result);
        }


        // api/suppliers/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SupplierUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var result = await _service.UpdateAsync(id, dto);

            return Ok(result);
        }


        // PATCH: api/suppliers/5
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, SupplierPatchDto dto)
        {
            var validationResult = await _patchValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var result = await _service.PatchAsync(id, dto);
            return Ok(result);
        }

        // DELETE: api/suppliers/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/suppliers/count
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalCount()
        {
            var count = await _service.GetTotalCountAsync();
            return Ok(count);
        }

        // GET: api/suppliers/recent/{count}
        [HttpGet("recent/{count}")]
        public async Task<IActionResult> GetRecent(int count)
        {
            var results = await _service.GetRecentAsync(count);

            return Ok(results);
        }
    }
}