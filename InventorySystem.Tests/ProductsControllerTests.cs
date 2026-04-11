using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation;
using FluentValidation.Results;
using ServerApp.Controllers;
using ServerApp.Services;
using SharedApp.Dto;

namespace InventorySystem.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly Mock<IValidator<ProductCreateDto>> _mockCreateValidator;
        private readonly Mock<IValidator<ProductUpdateDto>> _mockUpdateValidator;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _mockCreateValidator = new Mock<IValidator<ProductCreateDto>>();
            _mockUpdateValidator = new Mock<IValidator<ProductUpdateDto>>();
            _mockLogger = new Mock<ILogger<ProductsController>>();

            // Fake validators to satisfy the constructor
            var mockUpdate = new Mock<IValidator<ProductUpdateDto>>();
            var mockPatch = new Mock<IValidator<ProductPatchDto>>();

            _controller = new ProductsController(
           _mockService.Object,
           _mockCreateValidator.Object,
           _mockUpdateValidator.Object, // Use our mock here
           mockPatch.Object,
           _mockLogger.Object
            );
        }


        [Fact]
        public async Task GetAll_ReturnsOk_WithPaginationMetadata()
        {
            // Arrange
            var products = new List<ProductReadDto>
            {
                new ProductReadDto { ProductId = 1, Name = "P1" },
                new ProductReadDto { ProductId = 2, Name = "P2" }
            };

            // Tell Moq: Service returns 2 products and a total count of 20
            _mockService.Setup(s => s.GetPaginatedAsync(1, 10))
                        .ReturnsAsync((products, 20));

            // Act
            var result = await _controller.GetAll(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);


            var resultData = okResult.Value as dynamic;

            Assert.NotNull(resultData);
            Assert.Equal(20, (int)resultData?.GetType().GetProperty("TotalCount").GetValue(resultData, null));
            Assert.Equal(2, (int)resultData?.GetType().GetProperty("TotalPages").GetValue(resultData, null));
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenDataIsValid()
        {
            // Arrange
            var dto = new ProductCreateDto { Name = "Valid Camera", Price = 500 };
            var resultDto = new ProductReadDto { ProductId = 1, Name = "Valid Camera" };

            // Setup: Return a result with NO errors
            _mockCreateValidator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(resultDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, ((ProductReadDto)createdResult.Value!).ProductId);
        }

        [Fact]
        public async Task Create_ThrowsValidationException_WhenDataIsInvalid()
        {
            // Arrange 
            var dto = new ProductCreateDto { Name = "" }; // Invalid data

            // Setup: Return a result WITH errors
            var failures = new List<ValidationFailure> { new("Name", "Required") };
            var validationResult = new ValidationResult(failures);

            _mockCreateValidator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert 
            // This will now catch the exception thrown by your new controller logic
            await Assert.ThrowsAsync<ValidationException>(() => _controller.Create(dto));

            // Verify the service was never called
            _mockService.Verify(s => s.CreateAsync(It.IsAny<ProductCreateDto>()), Times.Never);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenDataIsValid()
        {
            // 1. Arrange
            int productId = 10;
            var dto = new ProductUpdateDto { Name = "Updated Tablet", Price = 3000 };
            var updatedDto = new ProductReadDto { ProductId = productId, Name = "Updated Tablet" };

            // Mock Validator: Return Success
            _mockUpdateValidator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Mock Service: Return the updated product
            _mockService.Setup(s => s.UpdateAsync(productId, dto))
                        .ReturnsAsync(updatedDto);

            // 2. Act
            var result = await _controller.Update(productId, dto);

            // 3. Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<ProductReadDto>(okResult.Value);
            Assert.Equal("Updated Tablet", model.Name);
        }

        [Fact]
        public async Task Update_ThrowsValidationException_WhenDataIsInvalid()
        {
            // 1. Arrange
            int productId = 10;
            var dto = new ProductUpdateDto { Name = "" }; // Invalid empty name

            var failures = new List<ValidationFailure> { new("Name", "Name is required") };

            // Mock Validator: Return Failure
            _mockUpdateValidator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            // 2. Act & 3. Assert
            await Assert.ThrowsAsync<ValidationException>(() => _controller.Update(productId, dto));

            // Verify service was never called
            _mockService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<ProductUpdateDto>()), Times.Never);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductExists()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(5))
                        .ReturnsAsync(new ProductReadDto { ProductId = 5, Name = "Existing Item" });

            // Act
            var result = await _controller.GetById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5, ((ProductReadDto)okResult.Value!).ProductId);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeletetionIsSuccessful()
        {
            // Arrange
            int productId = 99;

            // Tell Moq: When DeleteAsync(99) is called, just return (Success)
            _mockService.Setup(s => s.DeleteAsync(productId))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            // Verify that the result is an HTTP 204 (NoContent)
            Assert.IsType<NoContentResult>(result);

            // Double Check: Verify the service was actually called with the correct ID
            _mockService.Verify(s => s.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task Delete_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistentId = 500;

            // Tell Moq: When DeleteAsync(500) is called, throw an exception
            _mockService.Setup(s => s.DeleteAsync(nonExistentId))
                        .ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act & Assert
            // We expect the controller to pass this exception up to our Global Middleware
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Delete(nonExistentId));
        }

        [Fact]
        public async Task Search_ReturnsResults_WhenQueryProvided()
        {
            // Arrange
            var searchResults = new List<ProductReadDto> { new ProductReadDto { Name = "Search Item" } };
            _mockService.Setup(s => s.SearchAsync("Camera", "Electronics"))
                        .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search("Camera", "Electronics");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(okResult.Value);
            Assert.Single(items);
        }

        [Fact]
        public async Task GetCount_ReturnsTotalNumber()
        {
            // Arrange
            _mockService.Setup(s => s.GetTotalCountAsync()).ReturnsAsync(150);

            // Act
            var result = await _controller.GetCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(150, okResult.Value);
        }

        [Fact]
        public async Task GetLowStockCount_ReturnsCount()
        {
            // Arrange
            _mockService.Setup(s => s.GetLowStockCountAsync()).ReturnsAsync(3);

            // Act
            var result = await _controller.GetLowStockCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(3, okResult.Value);
        }
    }
}