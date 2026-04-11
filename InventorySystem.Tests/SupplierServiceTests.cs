using Moq;
using Xunit;
using ServerApp.Services;
using ServerApp.Repositories;
using SharedApp.Models;
using SharedApp.Dto;

namespace InventorySystem.Tests
{
    public class SupplierServiceTests
    {
        private readonly Mock<ISupplierRepository> _mockRepo;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _mockRepo = new Mock<ISupplierRepository>();
            _service = new SupplierService(_mockRepo.Object);
        }

        // This test verifies that the service correctly maps a Supplier entity to 
        // a SupplierReadDto and include the related products in the DTO
        [Fact]
        public async Task GetByIdAsync_ShouldReturnDetailedDto_WhenSupplierExists()
        {
            // Arrange: Create a fake supplier with products
            var supplierId = 1;
            var fakeSupplier = new Supplier
            {
                SupplierId = supplierId,
                Name = "Tech Corp",
                Location = "JHB",
                Email = "tech@test.com",
                Products = new List<Product>
            {
                new Product { Name = "Mouse", Price = 50, Category = "IT", SupplierId = 1 }
            }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(supplierId)).ReturnsAsync(fakeSupplier);

            // Act: Call the service method and get the result
            var result = await _service.GetByIdAsync(supplierId);

            // Assert: Verify that the returned DTO has the expected values and that 
            // the products were correctly mapped
            Assert.NotNull(result);
            Assert.Equal("Tech Corp", result.Name);
            Assert.Single(result.Products); // Verify exactly 1 product was mapped
            Assert.Equal("Mouse", result.Products[0].Name);
        }

        // This test verifies that the service throws a KeyNotFoundException 
        // when the repository returns null (i.e. supplier not found)
        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenRepositoryReturnsFalse()
        {
            // Arrange
            int idToDelete = 5;
            // Tell Moq: DeleteAsync returns FALSE (simulating a supplier with products or not found)
            _mockRepo.Setup(r => r.DeleteAsync(idToDelete)).ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(idToDelete));
            Assert.Contains("could not be deleted", ex.Message);
        }

        // This test verifies that the service correctly creates a new supplier 
        // and returns a SupplierReadDto with expected values
        [Fact]
        public async Task CreateAsync_ShouldReturnCorrectDto()
        {
            // Arrange
            var dto = new SupplierCreateDto { Name = "New Supp", Location = "DBN", Email = "supp@test.com" };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Email, result.Email);
            // Verify the repository's Add method was called
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Supplier>()), Times.Once);
        }
    }
}
