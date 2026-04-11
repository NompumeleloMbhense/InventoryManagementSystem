using Moq;
using ServerApp.Services;
using ServerApp.Repositories;
using System.ComponentModel;
using SharedApp.Models;
using SharedApp.Dto;

namespace InventorySystem.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // 1. Arrange: Create a "Fake" version of the Repository
            _mockRepo = new Mock<IProductRepository>();

            // 2. Arrange: Inject that fake repo into the actual ProductService
            _service = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenProductExists()
        {
            // Arrange
            var productId = 10;
            var fakeProduct = new Product
            {
                ProductId = productId,
                Name = "Gaming Mouse",
                Price = 500,
                Category = "Electronics",
                SupplierId = 1
            };

            // Tell Moq: "If GetByIdAsync(10) is called, return fakeProduct"
            _mockRepo.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(fakeProduct);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Gaming Mouse", result.Name);
            Assert.Equal(500, result.Price);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenProductIsNull()
        {
            // Arrange
            int nonExistentId = 999;
            _mockRepo.Setup(repo => repo.GetByIdAsync(nonExistentId))
                     .ReturnsAsync((Product?)null);

            // Act & Assert
            // This verifies that your service correctly throws an error when data is missing
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(nonExistentId));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenSupplierDoesNotExist()
        {
            // Arrange
            var dto = new ProductCreateDto { Name = "New Item", SupplierId = 55 };

            // Mock the check to say "No, Supplier 55 doesn't exist"
            _mockRepo.Setup(repo => repo.SupplierExistsAsync(55))
                     .ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
            Assert.Equal("Supplier does not exist", ex.Message);
        }
    }
}