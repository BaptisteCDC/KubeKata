using KubeKataApp.Domain.Entities;
using KubeKataApp.Domain.Repositories;
using System.Collections.Concurrent;

namespace KubeKataApp.Infrastructure.Repositories;

public class InMemoryAdminRepository : IAdminRepository
{
    private static readonly ConcurrentDictionary<Guid, AdminAccount> _storage = new();

    public Task<IEnumerable<AdminAccount>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<AdminAccount>>(_storage.Values);
    }

    public Task<AdminAccount?> GetByIdAsync(Guid id)
    {
        _storage.TryGetValue(id, out var account);
        return Task.FromResult(account);
    }

    public Task<AdminAccount?> GetByEmailAsync(string email)
    {
        var account = _storage.Values.FirstOrDefault(a => a.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(account);
    }

    public Task AddAsync(AdminAccount account)
    {
        _storage[account.Id] = account;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(AdminAccount account)
    {
        _storage[account.Id] = account;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _storage.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
