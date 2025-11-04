using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Validators;
using SharedApp.Models;
using SharedApp.Dto;
using ServerApp.Repositories;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _repo;
        private readonly IValidator<SupplierCreateDto> _createValidator;
        private readonly IValidator<SupplierUpdateDto> _updateValidator;
        private readonly IValidator<SupplierPatchDto> _patchValidator;

        public SuppliersController(
           ISupplierRepository repo,
           IValidator<SupplierCreateDto> createValidator,
           IValidator<SupplierUpdateDto> updateValidator,
           IValidator<SupplierPatchDto> patchValidator)
        {
            _repo = repo;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _patchValidator = patchValidator;
        }

        public IActionResult Index()
        {
            return Ok("Suppliers API is running...");
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { error = "PageNumber and PageSize must be greater than 0" });

            var suppliers = await _repo.GetPaginatedAsync(pageNumber, pageSize);
            var totalCount = await _repo.GetTotalCountAsync();

            var response = new
            {
                Data = suppliers.Select(s => new SupplierReadDto
                {
                    SupplierId = s.SupplierId,
                    Name = s.Name,
                    Location = s.Location,
                    Email = s.Email
                }),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        // GET: api/suppliers/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier is null)
                return NotFound(new { error = "Supplier not found" });


            // Map to SupplierWithProductsDto so that when you request
            // a supplier, the response will include the products
            var result = new SupplierWithProductsDto
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Location = supplier.Location,
                Email = supplier.Email,
                Products = supplier.Products.Select(p => new ProductForSupplierDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Category = p.Category,
                    Available = p.Available
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/suppliers
        [HttpPost]
        public async Task<IActionResult> Create(SupplierCreateDto dto)
        {
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var supplier = new Supplier
            {
                Name = dto.Name,
                Location = dto.Location,
                Email = dto.Email
            };

            await _repo.AddAsync(supplier);

            var result = new SupplierReadDto
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Location = supplier.Location,
                Email = supplier.Email
            };

            return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, result);
        }


        // api/suppliers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SupplierUpdateDto dto)
        {
            var existingSupplier = await _repo.GetByIdAsync(id);
            if (existingSupplier is null)
                return NotFound(new { error = "Supplier not found" });

            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            // Update fields, if provided
            existingSupplier.Name = dto.Name;
            existingSupplier.Location = dto.Location;
            existingSupplier.Email = dto.Email;

            await _repo.UpdateAsync(existingSupplier);

            var result = new SupplierReadDto
            {
                SupplierId = existingSupplier.SupplierId,
                Name = existingSupplier.Name,
                Location = existingSupplier.Location,
                Email = existingSupplier.Email
            };

            return Ok(result);
        }


        // PATCH: api/suppliers/5
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, SupplierPatchDto dto)
        {
            var existingSupplier = await _repo.GetByIdAsync(id);
            if (existingSupplier is null)
                return NotFound(new { error = "Supplier not found" });

            var validationResult = _patchValidator.Validate(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            if (dto.Name is not null)
                existingSupplier.Name = dto.Name;

            if (dto.Location is not null)
                existingSupplier.Location = dto.Location;

            if (dto.Email is not null)
                existingSupplier.Email = dto.Email;

            await _repo.UpdateAsync(existingSupplier);

            var result = new SupplierReadDto
            {
                SupplierId = existingSupplier.SupplierId,
                Name = existingSupplier.Name,
                Location = existingSupplier.Location,
                Email = existingSupplier.Email
            };

            return Ok(result);
        }

        // DELETE: api/suppliers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // call the repository to delete the supplier
                var deleted = await _repo.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Supplier not found" }); // Supplier not found

                // Successfully deleted
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the supplier has associated products
                return BadRequest(new { error = ex.Message });
            }


        }
    }
}