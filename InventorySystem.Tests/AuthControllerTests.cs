using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ServerApp.Controllers;
using ServerApp.Models;
using ServerApp.Services;
using SharedApp.Dto;

namespace InventorySystem.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<IJwtTokenService> _mockJwtService;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {

            _mockUserManager = MockUserManager();
            _mockConfig = new Mock<IConfiguration>();
            _mockJwtService = new Mock<IJwtTokenService>(); 

            _controller = new AuthController(_mockUserManager.Object, _mockConfig.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsNew()
        {
            // Arrange
            var dto = new RegisterDto { FullName = "John Doe", Email = "john@test.com", Password = "Password123!" };

            // Mock: Finding user by email returns NULL (meaning email is available)
            _mockUserManager.Setup(um => um.FindByEmailAsync(dto.Email))
                            .ReturnsAsync((AppUser?)null);

            // Mock: Creating user returns SUCCESS
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailExists()
        {
            // Arrange
            var dto = new RegisterDto { Email = "exists@test.com" };

            // Mock: Finding user returns an actual user (email taken)
            _mockUserManager.Setup(um => um.FindByEmailAsync(dto.Email))
                            .ReturnsAsync(new AppUser { UserName = "exists", FullName = "Existing User" });

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto { Email = "user@test.com", Password = "ValidPassword" };
            var user = new AppUser { Id = "1", Email = dto.Email, FullName = "User" };

            _mockUserManager.Setup(um => um.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);

            _mockJwtService.Setup(s => s.GenerateTokenAsync(user)).ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("fake-jwt-token", okResult?.Value?.ToString()!);
        }


        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsWrong()
        {
            // Arrange
            var dto = new LoginDto { Email = "user@test.com", Password = "WrongPassword" };
            var user = new AppUser { Email = dto.Email, FullName = "User" };

            _mockUserManager.Setup(um => um.FindByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Mock: Password check fails
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // Helper method to create a Mock UserManager
        private static Mock<UserManager<AppUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<AppUser>>();
            return new Mock<UserManager<AppUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }
    }
}