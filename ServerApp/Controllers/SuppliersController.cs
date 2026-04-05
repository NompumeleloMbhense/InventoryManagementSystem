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
            
            return Ok(new { Data = suppliers, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
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
            await _createValidator.ValidateAndThrowAsync(dto); // Throws if invalid
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.SupplierId }, result);
        }


        // api/suppliers/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SupplierUpdateDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);
            return Ok(await _service.UpdateAsync(id, dto));
        }


        // PATCH: api/suppliers/5
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, SupplierPatchDto dto)
        {
            await _patchValidator.ValidateAndThrowAsync(dto);
            return Ok(await _service.PatchAsync(id, dto));
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
        public async Task<ActionResult<int>> GetTotalCount()
        {
            var count = await _service.GetTotalCountAsync();
            return Ok(count);
        }

        // GET: api/suppliers/recent/{count}
        [HttpGet("recent/{count}")]
        public async Task<ActionResult> GetRecent(int count)
        {
            var results = await _service.GetRecentAsync(count);
            
            return Ok(results);
        }
    }
}