using KubeKataApp.Domain.ValueObjects;

namespace KubeKataApp.Domain.Entities;

public class AdminAccount
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AdminAccount(Guid id, string username, Email email, string passwordHash)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername)) throw new ArgumentException("Username cannot be empty.");
        Username = newUsername;
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail;
    }

    public void ResetPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
}
