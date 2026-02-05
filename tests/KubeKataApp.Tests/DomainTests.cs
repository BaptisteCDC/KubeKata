using KubeKataApp.Domain.Entities;
using KubeKataApp.Domain.ValueObjects;
using Xunit;

namespace KubeKataApp.Tests.Domain;

public class AdminAccountTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "testuser";
        var email = new Email("test@example.com");
        var passwordHash = "hashed_password";

        // Act
        var admin = new AdminAccount(id, username, email, passwordHash);

        // Assert
        Assert.Equal(id, admin.Id);
        Assert.Equal(username, admin.Username);
        Assert.Equal(email, admin.Email);
        Assert.Equal(passwordHash, admin.PasswordHash);
        Assert.True(admin.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UpdateUsername_ShouldChangeUsername()
    {
        // Arrange
        var admin = new AdminAccount(Guid.NewGuid(), "old", new Email("t@t.com"), "pass");

        // Act
        admin.UpdateUsername("new");

        // Assert
        Assert.Equal("new", admin.Username);
    }

    [Fact]
    public void UpdateUsername_ShouldThrowException_WhenEmpty()
    {
        // Arrange
        var admin = new AdminAccount(Guid.NewGuid(), "old", new Email("t@t.com"), "pass");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => admin.UpdateUsername(""));
    }
}

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    public void ValidEmail_ShouldNotThrow(string email)
    {
        // Act
        var emailVo = new Email(email);

        // Assert
        Assert.Equal(email.ToLowerInvariant(), emailVo.Value);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("")]
    [InlineData(null)]
    public void InvalidEmail_ShouldThrowException(string? email)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(email!));
    }
}
