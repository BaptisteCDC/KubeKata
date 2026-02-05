using KubeKataApp.Application.DTOs;
using KubeKataApp.Application.Services;
using KubeKataApp.Domain.Entities;
using KubeKataApp.Domain.Repositories;
using KubeKataApp.Domain.ValueObjects;
using Moq;
using Xunit;

namespace KubeKataApp.Tests.Application;

public class AdminAppServiceTests
{
    private readonly Mock<IAdminRepository> _repoMock;
    private readonly AdminAppService _service;

    public AdminAppServiceTests()
    {
        _repoMock = new Mock<IAdminRepository>();
        _service = new AdminAppService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAdminsAsync_ShouldReturnDtos()
    {
        // Arrange
        var admins = new List<AdminAccount>
        {
            new AdminAccount(Guid.NewGuid(), "user1", new Email("u1@t.com"), "hash"),
            new AdminAccount(Guid.NewGuid(), "user2", new Email("u2@t.com"), "hash")
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(admins);

        // Act
        var result = await _service.GetAllAdminsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.Username == "user1");
    }

    [Fact]
    public async Task CreateAdminAsync_ShouldSaveInRepo()
    {
        // Arrange
        var createDto = new CreateAdminDto("newuser", "new@test.com", "pass");

        // Act
        var result = await _service.CreateAdminAsync(createDto);

        // Assert
        _repoMock.Verify(r => r.AddAsync(It.IsAny<AdminAccount>()), Times.Once);
        Assert.Equal("newuser", result.Username);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnTrue_WhenAccountExists()
    {
        // Arrange
        var admin = new AdminAccount(Guid.NewGuid(), "test", new Email("test@t.com"), "hash");
        _repoMock.Setup(r => r.GetByEmailAsync("test@t.com")).ReturnsAsync(admin);

        // Act
        var result = await _service.RequestPasswordResetAsync("test@t.com");

        // Assert
        Assert.True(result);
    }
}
