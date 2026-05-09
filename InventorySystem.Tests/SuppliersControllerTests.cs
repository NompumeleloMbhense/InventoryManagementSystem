using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using ServerApp.Controllers;
using ServerApp.Services;
using SharedApp.Dto;

namespace InventorySystem.Tests
{
    public class SuppliersControllerTests
    {
        private readonly Mock<ISupplierService> _mockService;
        private readonly Mock<IValidator<SupplierCreateDto>> _mockCreateVal;
        private readonly Mock<IValidator<SupplierUpdateDto>> _mockUpdateVal;
        private readonly SuppliersController _controller;

        public SuppliersControllerTests()
        {
            _mockService = new Mock<ISupplierService>();
            _mockCreateVal = new Mock<IValidator<SupplierCreateDto>>();
            _mockUpdateVal = new Mock<IValidator<SupplierUpdateDto>>();

            
            _controller = new SuppliersController(
                _mockService.Object,
                _mockCreateVal.Object,
                _mockUpdateVal.Object,
                new Mock<IValidator<SupplierPatchDto>>().Object
            );
        }

        [Fact]
        public async Task GetAll_ReturnsPaginatedData()
        {
            // Arrange
            var list = new List<SupplierReadDto> { new SupplierReadDto { Name = "S1" } };
            _mockService.Setup(s => s.GetPaginatedAsync(1, 10, null))
                        .ReturnsAsync((list, 1));

            // Act
            var result = await _controller.GetAll(1, 10, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithDataAndMetadata()
        {
            // Arrange
            var suppliers = new List<SupplierReadDto> { new SupplierReadDto { Name = "Supp 1" } };
            _mockService.Setup(s => s.GetPaginatedAsync(1, 10, null))
                        .ReturnsAsync((suppliers, 1));

            // Act
            var result = await _controller.GetAll(1, 10, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Verify anonymous object properties using reflection or casting to dynamic
            var value = okResult.Value as dynamic;
            Assert.NotNull(value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenSupplierExists()
        {
            // Arrange
            var dto = new SupplierWithProductsDto { SupplierId = 1, Name = "Test Supp" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, ((SupplierReadDto)okResult.Value!).SupplierId);
        }

        [Fact]
        public async Task GetById_BubblesException_WhenNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(99)).ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(99));
        }


        [Fact]
        public async Task Create_ReturnsCreated_WhenDataIsValid()
        {
            // Arrange
            var dto = new SupplierCreateDto { Name = "Global Tech", Email = "info@global.com" };
            var resultDto = new SupplierReadDto { SupplierId = 1, Name = "Global Tech" };

            _mockCreateVal.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new ValidationResult()); // Valid

            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(resultDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, ((SupplierReadDto)createdResult.Value!).SupplierId);
        }

        [Fact]
        public async Task Create_ThrowsValidationException_WhenNameIsEmpty()
        {
            // Arrange
            var dto = new SupplierCreateDto { Name = "" };
            var failures = new List<ValidationFailure> { new("Name", "Required") };

            _mockCreateVal.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new ValidationResult(failures)); // Invalid

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _controller.Create(dto));
            _mockService.Verify(s => s.CreateAsync(It.IsAny<SupplierCreateDto>()), Times.Never);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            int supplierId = 10;
            var dto = new SupplierUpdateDto
            {
                Name = "Updated Tech World",
                Location = "Cape Town",
                Email = "new@techworld.com"
            };

            var updatedResultDto = new SupplierReadDto
            {
                SupplierId = supplierId,
                Name = "Updated Tech World",
                Location = "Cape Town",
                Email = "new@techworld.com"
            };

            
            _mockUpdateVal.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ValidationResult()); 

            
            _mockService.Setup(s => s.UpdateAsync(supplierId, dto))
                        .ReturnsAsync(updatedResultDto);

            // Act 
            var result = await _controller.Update(supplierId, dto);

            // Assert
            // Verify the HTTP status code is 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Verify the data returned is what we expected
            var model = Assert.IsType<SupplierReadDto>(okResult.Value);
            Assert.Equal("Updated Tech World", model.Name);
            Assert.Equal(supplierId, model.SupplierId);

            // Verify the service was actually called exactly once
            _mockService.Verify(s => s.UpdateAsync(supplierId, dto), Times.Once);
        }

        [Fact]
        public async Task Update_ThrowsValidationException_WhenInvalid()
        {
            // Arrange
            var dto = new SupplierUpdateDto { Name = "" };
            var failures = new List<ValidationFailure> { new("Name", "Required") };
            _mockUpdateVal.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _controller.Update(1, dto));
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_OnSuccess()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetTotalCount_ReturnsNumber()
        {
            // Arrange
            _mockService.Setup(s => s.GetTotalCountAsync()).ReturnsAsync(50);

            // Act
            var result = await _controller.GetTotalCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(50, okResult.Value);
        }

        [Fact]
        public async Task GetRecent_ReturnsList()
        {
            // Arrange
            var list = new List<SupplierReadDto> { new SupplierReadDto { Name = "Recent 1" } };
            _mockService.Setup(s => s.GetRecentAsync(3)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetRecent(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<SupplierReadDto>>(okResult.Value);
            Assert.Single(data);
        }
    }
}